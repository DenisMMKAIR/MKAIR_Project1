import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManometrClient, Manometr1VerificationDto, DeviceLocation } from '../../api-client';
import { ManometrsService } from '../../services/manometrs.service';
import { DatesFilterComponent } from '../../shared/dates-filter/dates-filter.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-manometrs-page',
  standalone: true,
  templateUrl: './manometrs.page.html',
  styleUrls: ['./manometrs.page.scss'],
  imports: [NgFor, NgIf, DatePipe, FormsModule, DatesFilterComponent, DecimalPipe],
  providers: [ManometrClient],
})
export class ManometrsPage implements OnInit {
  private readonly manometrClient = inject(ManometrClient);
  private readonly manometrsService = inject(ManometrsService);

  public manometrs: Manometr1VerificationDto[] = [];
  public loading = false;
  public exportLoading = false;
  public error: string | null = null;
  public infoMessage: string | null = null;
  public successMessage: string | null = null;
  public selectedRows = new Set<number>();
  private exportSubscription: Subscription | null = null;

  ngOnInit(): void {
    this.manometrsService.setPageChangeCallback(() => this.loadManometrs());
    this.loadManometrs();
  }

  // Public getters for template binding
  public get pagination() {
    return this.manometrsService.getPagination();
  }

  public get yearMonthFilter() {
    return this.manometrsService.getYearMonthFilter();
  }

  public set yearMonthFilter(value: string | null) {
    this.manometrsService.setYearMonthFilter(value);
  }

  public get yearMonthOptions() {
    return this.manometrsService.getYearMonthOptions();
  }

  public get deviceTypeNumberFilter() {
    return this.manometrsService.getDeviceTypeNumberFilter();
  }

  public set deviceTypeNumberFilter(value: string | null) {
    this.manometrsService.setDeviceTypeNumberFilter(value);
  }

  public get deviceSerialFilter() {
    return this.manometrsService.getDeviceSerialFilter();
  }

  public set deviceSerialFilter(value: string | null) {
    this.manometrsService.setDeviceSerialFilter(value);
  }

  public get locationFilter() {
    return this.manometrsService.getLocationFilter();
  }

  public set locationFilter(value: string | null) {
    this.manometrsService.setLocationFilter(value);
  }

  // Data loading methods
  public loadManometrs(): void {
    this.loading = true;
    this.error = null;
    
    // Prepare filter values
    const deviceTypeNumber = this.deviceTypeNumberFilter || null;
    const deviceSerial = this.deviceSerialFilter || null;
    const yearMonth = !this.yearMonthFilter || this.yearMonthFilter === 'all' ? null : this.yearMonthFilter;
    const location = this.locationFilter === null || this.locationFilter === 'all' ? null : this.locationFilter as DeviceLocation;
    
    this.manometrClient.getVerifications(
      this.pagination.currentPage, 
      this.pagination.pageSize,
      deviceTypeNumber,
      deviceSerial,
      yearMonth,
      location
    ).subscribe({
      next: (result) => {
        if (result.data) {
          this.manometrs = result.data.items ?? [];
          this.manometrsService.updatePaginationFromData(result.data);
        } else {
          this.manometrs = [];
          this.manometrsService.resetPagination();
        }
        this.loading = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось загрузить манометры';
        this.loading = false;
      },
    });
  }

  public onYearMonthFilterChange(): void {
    this.manometrsService.resetToFirstPage();
  }

  public onLocationFilterChange(): void {
    this.manometrsService.resetToFirstPage();
  }

  // Note: Device filter change handlers are no longer needed
  // as throttling is handled automatically by the service

  public exportToPdf(): void {
    if (this.selectedRows.size === 0 || this.exportLoading) {
      return;
    }
    this.exportLoading = true;
    this.error = null;
    this.infoMessage = null;
    this.successMessage = null;
    const selectedManometrs = Array.from(this.selectedRows).map(index => this.manometrs[index]);
    const ids = selectedManometrs.map(m => m.id).filter((id): id is string => typeof id === 'string');
    if (ids.length === 0) {
      this.error = 'Нет выбранных записей для экспорта.';
      this.exportLoading = false;
      return;
    }
    this.exportSubscription = this.manometrClient.exportToPdf(ids).subscribe({
      next: (response: any) => {
        if (response?.message) {
          this.error = null;
          this.successMessage = response.message;
          setTimeout(() => {
            this.successMessage = null;
          }, 5000);
        } else if (response?.error) {
          this.error = response.error;
        } else {
          this.error = 'Неожиданный ответ от сервера.';
        }
        this.exportLoading = false;
        this.selectedRows.clear();
        this.exportSubscription = null;
      },
      error: (err: any) => {
        this.error = err?.error?.error || err?.error?.message || err?.message || 'Ошибка при экспорте PDF.';
        this.exportLoading = false;
        this.exportSubscription = null;
      }
    });
  }

  public exportAllToPdf(): void {
    if (this.exportLoading) {
      return;
    }
    this.exportLoading = true;
    this.error = null;
    this.infoMessage = null;
    this.successMessage = null;
    this.exportSubscription = this.manometrClient.exportAllToPdf().subscribe({
      next: (response: any) => {
        if (response?.message) {
          this.error = null;
          this.successMessage = response.message;
          setTimeout(() => {
            this.successMessage = null;
          }, 5000);
        } else if (response?.error) {
          this.error = response.error;
        } else {
          this.error = 'Неожиданный ответ от сервера.';
        }
        this.exportLoading = false;
        this.exportSubscription = null;
      },
      error: (err: any) => {
        this.error = err?.error?.error || err?.error?.message || err?.message || 'Ошибка при экспорте PDF.';
        this.exportLoading = false;
        this.exportSubscription = null;
      }
    });
  }

  public cancelExport(): void {
    if (this.exportSubscription) {
      this.exportSubscription.unsubscribe();
      this.exportSubscription = null;
      this.exportLoading = false;
      this.infoMessage = 'Экспорт отменён пользователем.';
    }
  }

  public toggleRowSelection(index: number, event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      this.selectedRows.add(index);
    } else {
      this.selectedRows.delete(index);
    }
  }

  public toggleSelectAll(event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      // Select all rows
      this.selectedRows.clear();
      for (let i = 0; i < this.manometrs.length; i++) {
        this.selectedRows.add(i);
      }
    } else {
      // Deselect all rows
      this.selectedRows.clear();
    }
  }

  public isAllSelected(): boolean {
    return this.selectedRows.size === this.manometrs.length && this.manometrs.length > 0;
  }

  // Data formatting methods
  public formatRangeAccuracy(m: Manometr1VerificationDto): string {
    const range = (m.measurementMin != null && m.measurementMax != null && m.measurementUnit) 
      ? `${m.measurementMin}–${m.measurementMax} ${m.measurementUnit}` 
      : '-';
    const accuracyClass = m.validError != null ? `±${m.validError}` : '-';
    
    return `${range}\n${accuracyClass}`;
  }

  public formatDeviceValues(m: Manometr1VerificationDto): string {
    if (m.deviceValues && m.deviceValues.length > 0) {
      return this.formatTableData(m.deviceValues);
    }
    return '-';
  }

  public formatEtalonValues(m: Manometr1VerificationDto): string {
    if (m.etalonValues && m.etalonValues.length > 0) {
      return this.formatTableData(m.etalonValues);
    }
    return '-';
  }

  public formatActualError(m: Manometr1VerificationDto): string {
    if (m.actualError && m.actualError.length > 0) {
      return this.formatTableData(m.actualError);
    }
    return '-';
  }

  public formatActualVariation(m: Manometr1VerificationDto): string {
    if (m.actualVariation && m.actualVariation.length > 0) {
      // Convert 1D array to 2D array for table formatting
      const data2D = [m.actualVariation];
      return this.formatTableData(data2D);
    }
    return '-';
  }

  // Private utility methods
  private formatTableData(data: number[][]): string {
    if (!data || data.length === 0) return '-';
    
    const maxColumns = Math.max(...data.map(row => row.length));
    const tableRows: string[] = [];
    
    for (let colIndex = 0; colIndex < maxColumns; colIndex++) {
      const rowValues: string[] = [];
      for (let rowIndex = 0; rowIndex < data.length; rowIndex++) {
        const value = data[rowIndex][colIndex];
        rowValues.push(this.formatNumber(value));
      }
      tableRows.push(rowValues.join(' | '));
    }
    
    return tableRows.join('\n');
  }

  private formatNumber(value: number): string {
    if (value === null || value === undefined || isNaN(value)) {
      return '-';
    }
    // Format to 3 decimal places and remove trailing zeros
    const formatted = value.toFixed(3);
    return formatted.replace(/\.?0+$/, '');
  }

  public deleteSelectedVerifications(): void {
    if (this.selectedRows.size === 0 || this.loading || this.exportLoading) {
      return;
    }
    if (!confirm('Вы уверены, что хотите удалить выбранные поверки?')) {
      return;
    }
    this.loading = true;
    this.error = null;
    this.successMessage = null;
    const selectedManometrs = Array.from(this.selectedRows).map(index => this.manometrs[index]);
    const ids = selectedManometrs.map(m => m.id).filter((id): id is string => typeof id === 'string');
    if (ids.length === 0) {
      this.error = 'Нет выбранных записей для удаления.';
      this.loading = false;
      return;
    }
    this.manometrClient.deleteVerifications(ids).subscribe({
      next: (result: any) => {
        if (result?.error) {
          this.error = result.error;
        } else {
          this.successMessage = result?.message || 'Выбранные поверки удалены.';
        }
        this.selectedRows.clear();
        this.loading = false;
        this.loadManometrs();
      },
      error: (err: any) => {
        this.error = err?.error?.error || err?.error?.message || err?.message || 'Ошибка при удалении поверок.';
        this.selectedRows.clear();
        this.loading = false;
        this.loadManometrs();
      }
    });
  }
}