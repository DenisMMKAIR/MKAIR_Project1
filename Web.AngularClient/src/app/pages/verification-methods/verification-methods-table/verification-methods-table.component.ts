import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VerificationMethodsClient, VerificationMethodDTO } from '../../../api-client';
import { VerificationMethodsService } from '../../../services/verification-methods.service';

@Component({
  selector: 'app-verification-methods-table',
  standalone: true,
  templateUrl: './verification-methods-table.component.html',
  styleUrls: ['./verification-methods-table.component.scss'],
  imports: [NgFor, NgIf, FormsModule],
  providers: [VerificationMethodsClient],
})
export class VerificationMethodsTableComponent implements OnInit {
  private readonly verificationMethodsClient = inject(VerificationMethodsClient);
  private readonly verificationMethodsService = inject(VerificationMethodsService);
  private readonly key = 'verification' as const;

  public verificationMethods: VerificationMethodDTO[] = [];
  public loading = false;
  public error: string | null = null;

  ngOnInit(): void {
    this.verificationMethodsService.setPageChangeCallback(this.key, () => this.loadVerificationMethods());
    this.loadVerificationMethods();
  }

  public loadVerificationMethods(): void {
    this.loading = true;
    this.error = null;
    this.verificationMethodsClient.getVerificationMethods(
      this.pagination.currentPage,
      this.pagination.pageSize
    ).subscribe({
      next: (result) => {
        if (result.data) {
          this.verificationMethods = result.data.items ?? [];
          this.verificationMethodsService.updatePaginationFromData(this.key, result.data);
        } else {
          this.verificationMethods = [];
          this.verificationMethodsService.resetPagination(this.key);
        }
        this.loading = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось загрузить методы поверки';
        this.loading = false;
      },
    });
  }

  public get pagination() {
    return this.verificationMethodsService.getPagination(this.key);
  }

  public reload() {
    this.loadVerificationMethods();
  }
} 