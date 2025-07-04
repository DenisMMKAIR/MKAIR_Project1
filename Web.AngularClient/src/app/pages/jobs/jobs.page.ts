import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import {
  InitialVerificationJobsClient,
  InitialVerificationJob,
} from '../../api-client';
import { FormsModule } from '@angular/forms';
import { JobsService } from '../../services/jobs.service';

@Component({
  selector: 'app-jobs-page',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule, HttpClientModule],
  templateUrl: './jobs.page.html',
  styleUrls: ['./jobs.page.scss'],
  providers: [InitialVerificationJobsClient],
})
export class JobsPage implements OnInit {
  private readonly jobsClient = inject(InitialVerificationJobsClient);
  private readonly jobsService = inject(JobsService);

  public jobs: InitialVerificationJob[] = [];
  public loading = false;
  public error: string | null = null;
  public newYear: number | null = null;
  public newMonth: number | null = null;
  public addLoading = false;
  public addError: string | null = null;
  public deleteError: string | null = null;

  ngOnInit(): void {
    this.jobsService.setPageChangeCallback(() => this.loadJobs());
    this.loadJobs();
  }

  public loadJobs(): void {
    this.loading = true;
    this.error = null;
    this.addError = null;
    this.deleteError = null;
    
    this.jobsClient.getJobs(this.pagination.currentPage, this.pagination.pageSize).subscribe({
      next: (result) => {
        if (result.data) {
          this.jobs = result.data.items ?? [];
          this.jobsService.updatePaginationFromData(result.data);
        } else {
          this.jobs = [];
          this.jobsService.resetPagination();
        }
        this.loading = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось загрузить задания';
        this.loading = false;
      },
    });
  }

  public addJob(): void {
    if (this.newYear == null || this.newMonth == null) return;
    this.addLoading = true;
    this.addError = null;
    this.jobsClient.addJob(this.newYear, this.newMonth).subscribe({
      next: (res) => {
        if (res.error) {
          this.addError = res.error;
          this.addLoading = false;
          return;
        }
        this.addLoading = false;
        this.newYear = null;
        this.newMonth = null;
        this.loadJobs();
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.addError = msg || 'Не удалось добавить задание';
        this.addLoading = false;
      },
    });
  }

  public deleteJob(id?: string): void {
    if (!id) return;
    this.deleteError = null;
    this.jobsClient.deleteJob(id).subscribe({
      next: (res) => {
        if (res.error) {
          this.deleteError = res.error;
          return;
        }
        this.loadJobs();
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.deleteError = msg || 'Не удалось удалить задание';
      },
    });
  }

  public get pagination() {
    return this.jobsService.getPagination();
  }
}
