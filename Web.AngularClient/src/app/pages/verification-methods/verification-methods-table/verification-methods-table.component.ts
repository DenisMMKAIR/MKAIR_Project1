import { Component, inject, OnInit, Output, EventEmitter } from '@angular/core';
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

  @Output() reloadPossible = new EventEmitter<void>();

  public verificationMethods: VerificationMethodDTO[] = [];
  public loading = false;
  public error: string | null = null;
  public successMessage: string | null = null;
  public aliasInputs: { [id: string]: string } = {};
  public aliasLoading: { [id: string]: boolean } = {};
  public deletingMethods: { [id: string]: boolean } = {};

  ngOnInit(): void {
    this.verificationMethodsService.setPageChangeCallback(this.key, () => this.loadVerificationMethods());
    this.loadVerificationMethods();
  }

  public loadVerificationMethods(): void {
    this.loading = true;
    this.error = null;
    this.successMessage = null;
    this.verificationMethodsClient.getVerificationMethods(this.pagination.currentPage, this.pagination.pageSize).subscribe({
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

  public get pagination() { return this.verificationMethodsService.getPagination(this.key); }

  public reload() { this.loadVerificationMethods(); }

  public downloadFile(methodId: string, fileName: string) {
    this.verificationMethodsClient.downloadFile(fileName).subscribe({
      next: (response) => {
        const blob = new Blob([response.data], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        const msg = err?.error?.message || err?.message || '';
        this.error = 'Ошибка загрузки файла: ' + msg;
        setTimeout(() => { this.error = null; }, 5000);
      },
    });
  }

  public addAliasOnEnter(methodId: string, event?: any) {
    if (!event || event.key === 'Enter') {
      const input = (this.aliasInputs[methodId] || '').trim();
      if (!input || this.aliasLoading[methodId]) return;
      this.aliasLoading[methodId] = true;
      this.error = null;
      this.successMessage = null;
      this.verificationMethodsClient.addAliases([input], methodId).subscribe({
        next: (result) => {
          this.aliasInputs[methodId] = '';
          this.aliasLoading[methodId] = false;
          this.successMessage = result.message || 'Псевдоним успешно добавлен';
          setTimeout(() => { this.successMessage = null; }, 5000);
          this.reload();
          this.reloadPossible.emit();
        },
        error: (err) => {
          this.aliasLoading[methodId] = false;
          const msg = err?.error?.message || err?.message || '';
          this.error = 'Ошибка добавления псевдонима: ' + msg;
          setTimeout(() => { this.error = null; }, 5000);
        },
      });
    }
  }

  public deleteVerificationMethod(methodId: string): void {
    if (this.deletingMethods[methodId]) return;
    this.deletingMethods[methodId] = true;
    this.error = null;
    this.successMessage = null;
    this.verificationMethodsClient.deleteVerificationMethod(methodId).subscribe({
      next: (result) => {
        this.deletingMethods[methodId] = false;
        this.successMessage = result.message || 'Метод поверки успешно удален';
        setTimeout(() => { this.successMessage = null; }, 5000);
        this.reload();
        this.reloadPossible.emit();
      },
      error: (err) => {
        this.deletingMethods[methodId] = false;
        const msg = err?.error?.error || err?.error?.message || err?.message || '';
        this.error = 'Ошибка удаления метода поверки: ' + msg;
        setTimeout(() => { this.error = null; }, 5000);
      },
    });
  }

  public clearMessages(): void { this.error = null; this.successMessage = null; }
} 