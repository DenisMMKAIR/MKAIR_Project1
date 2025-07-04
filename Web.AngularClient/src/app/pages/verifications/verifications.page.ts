import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { InitialVerificationsClient, InitialVerificationDto, PaginatedListOfInitialVerificationDto } from '../../api-client';
import { VerificationsService } from '../../services/verifications.service';

@Component({
  selector: 'app-verifications-page',
  standalone: true,
  templateUrl: './verifications.page.html',
  styleUrls: ['./verifications.page.scss'],
  imports: [NgFor, NgIf, HttpClientModule, DatePipe, FormsModule],
  providers: [InitialVerificationsClient],
})
export class VerificationsPage implements OnInit {
  private readonly verificationsClient = inject(InitialVerificationsClient);
  private readonly verificationsService = inject(VerificationsService);

  public verifications: InitialVerificationDto[] = [];
  public loading = false;
  public error: string | null = null;

  ngOnInit(): void {
    this.verificationsService.setPageChangeCallback(() => this.loadVerifications());
    this.loadVerifications();
  }

  public loadVerifications(): void {
    this.loading = true;
    this.error = null;
    const yearMonthFilter = this.yearMonthFilter === null || this.yearMonthFilter === 'all'
      ? null 
      : this.yearMonthFilter;
    
    this.verificationsClient.getVerifications(this.pagination.currentPage, this.pagination.pageSize, yearMonthFilter).subscribe({
      next: (result) => {
        if (result.data) {
          this.verifications = result.data.items ?? [];
          this.verificationsService.updatePaginationFromData(result.data);
        } else {
          this.verifications = [];
          this.verificationsService.resetPagination();
        }
        this.loading = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось загрузить поверки';
        this.loading = false;
      },
    });
  }

  public onYearMonthFilterChange(): void {
    this.verificationsService.resetToFirstPage();
  }

  public get pagination() {
    return this.verificationsService.getPagination();
  }

  public get yearMonthFilter() {
    return this.verificationsService.getYearMonthFilter();
  }

  public set yearMonthFilter(value: string | null) {
    this.verificationsService.setYearMonthFilter(value);
  }

  public get yearMonthOptions() {
    return this.verificationsService.getYearMonthOptions();
  }
}