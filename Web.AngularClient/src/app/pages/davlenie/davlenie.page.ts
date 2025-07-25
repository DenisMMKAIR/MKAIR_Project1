import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DavlenieClient, Davlenie1VerificationDTO, DeviceLocation } from '../../api-client';
import { DavlenieService } from '../../services/davlenie.service';
import { DatesFilterComponent } from '../../shared/dates-filter/dates-filter.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-davlenie-page',
  standalone: true,
  templateUrl: './davlenie.page.html',
  styleUrls: ['./davlenie.page.scss'],
  imports: [NgFor, NgIf, DatePipe, FormsModule, DatesFilterComponent, DecimalPipe],
  providers: [DavlenieClient],
})
export class DavleniePage implements OnInit {
  private readonly davlenieClient = inject(DavlenieClient);
  private readonly davlenieService = inject(DavlenieService);

  public davlenie: Davlenie1VerificationDTO[] = [];
  public loading = false;
  public exportLoading = false;
  public error: string | null = null;
  public infoMessage: string | null = null;
  public successMessage: string | null = null;
  public selectedRows = new Set<number>();
  private exportSubscription: Subscription | null = null;

  ngOnInit(): void {
    this.davlenieService.setPageChangeCallback(() => this.loadDavlenie());
    this.loadDavlenie();
  }

  // Public getters for template binding
  public get pagination() {
    return this.davlenieService.getPagination();
  }

  public get yearMonthFilter() {
    return this.davlenieService.getYearMonthFilter();
  }

  public set yearMonthFilter(value: string | null) {
    this.davlenieService.setYearMonthFilter(value);
  }

  public get yearMonthOptions() {
    return this.davlenieService.getYearMonthOptions();
  }

  public get deviceTypeNumberFilter() {
    return this.davlenieService.getDeviceTypeNumberFilter();
  }

  public set deviceTypeNumberFilter(value: string | null) {
    this.davlenieService.setDeviceTypeNumberFilter(value);
  }

  public get deviceSerialFilter() {
    return this.davlenieService.getDeviceSerialFilter();
  }

  public set deviceSerialFilter(value: string | null) {
    this.davlenieService.setDeviceSerialFilter(value);
  }

  public get locationFilter() {
    return this.davlenieService.getLocationFilter();
  }

  public set locationFilter(value: string | null) {
    this.davlenieService.setLocationFilter(value);
  }

  // Data loading methods
  public loadDavlenie(): void {
    this.loading = true;
    this.error = null;

    // Prepare filter values
    const deviceTypeNumber = this.deviceTypeNumberFilter || null;
    const deviceSerial = this.deviceSerialFilter || null;
    const yearMonth = !this.yearMonthFilter || this.yearMonthFilter === 'all' ? null : this.yearMonthFilter;
    const location = this.locationFilter === null || this.locationFilter === 'all' ? null : this.locationFilter as DeviceLocation;

    this.davlenieClient.getVerifications(
      this.pagination.currentPage,
      this.pagination.pageSize,
      deviceTypeNumber,
      deviceSerial,
      yearMonth,
      location
    ).subscribe({
      next: (result) => {
        if (result.data) {
          this.davlenie = result.data.items ?? [];
          this.davlenieService.updatePaginationFromData(result.data);
        } else {
          this.davlenie = [];
          this.davlenieService.resetPagination();
        }
        this.loading = false;
      },
      error: (err) => {
        const msg = err?.error?.error || err?.error?.message || err?.message;
        this.error = msg || 'Не удалось загрузить данные';
        this.loading = false;
      },
    });
  }

  public onYearMonthFilterChange(): void {
    this.davlenieService.resetToFirstPage();
  }

  public onLocationFilterChange(): void {
    this.davlenieService.resetToFirstPage();
  }

  public exportToPdf(): void {
    if (this.selectedRows.size === 0 || this.exportLoading) {
      return;
    }
    this.exportLoading = true;
    this.error = null;
    this.infoMessage = null;
    this.successMessage = null;
    const selectedDavlenie = Array.from(this.selectedRows).map(index => this.davlenie[index]);
    const ids = selectedDavlenie.map(m => m.id).filter((id): id is string => typeof id === 'string');
    if (ids.length === 0) {
      this.error = 'Нет выбранных записей для экспорта.';
      this.exportLoading = false;
      return;
    }
    this.exportSubscription = this.davlenieClient.exportToPdf(ids).subscribe({
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
    this.exportSubscription = this.davlenieClient.exportAllToPdf().subscribe({
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
      for (let i = 0; i < this.davlenie.length; i++) {
        this.selectedRows.add(i);
      }
    } else {
      // Deselect all rows
      this.selectedRows.clear();
    }
  }

  public isAllSelected(): boolean {
    return this.selectedRows.size === this.davlenie.length && this.davlenie.length > 0;
  }

  // Data formatting methods
  public formatRangeAccuracy(m: Davlenie1VerificationDTO): string {
    const range = (m.measurementMin != null && m.measurementMax != null && m.measurementUnit)
      ? `${m.measurementMin}–${m.measurementMax} ${m.measurementUnit}`
      : '-';
    const accuracyClass = m.validError != null ? `±${m.validError}` : '-';
    return `${range}\n${accuracyClass}`;
  }

  public formatDeviceValues(m: Davlenie1VerificationDTO): string {
    if (m.deviceValues && m.deviceValues.length > 0) {
      return this.formatTableData(m.deviceValues);
    }
    return '-';
  }

  public formatEtalonValues(m: Davlenie1VerificationDTO): string {
    if (m.etalonValues && m.etalonValues.length > 0) {
      // Davlenie etalonValues is 1D array, convert to 2D for table
      return this.formatTableData([m.etalonValues]);
    }
    return '-';
  }

  public formatActualError(m: Davlenie1VerificationDTO): string {
    if (m.actualError && m.actualError.length > 0) {
      return this.formatTableData(m.actualError);
    }
    return '-';
  }

  public formatActualVariation(m: Davlenie1VerificationDTO): string {
    if (m.variations && m.variations.length > 0) {
      // Convert 1D array to 2D array for table formatting
      const data2D = [m.variations];
      return this.formatTableData(data2D);
    }
    return '-';
  }

  // Add helper for pressureInputs if needed
  public formatPressureInputs(m: Davlenie1VerificationDTO): string {
    return m.pressureInputs && m.pressureInputs.length > 0 ? m.pressureInputs.join(', ') : '-';
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
    const selectedDavlenie = Array.from(this.selectedRows).map(index => this.davlenie[index]);
    const ids = selectedDavlenie.map(m => m.id).filter((id): id is string => typeof id === 'string');
    if (ids.length === 0) {
      this.error = 'Нет выбранных записей для удаления.';
      this.loading = false;
      return;
    }
    this.davlenieClient.deleteVerifications(ids).subscribe({
      next: (result: any) => {
        if (result?.error) {
          this.error = result.error;
        } else {
          this.successMessage = result?.message || 'Выбранные поверки удалены.';
        }
        this.selectedRows.clear();
        this.loading = false;
        this.loadDavlenie();
      },
      error: (err: any) => {
        this.error = err?.error?.error || err?.error?.message || err?.message || 'Ошибка при удалении поверок.';
        this.selectedRows.clear();
        this.loading = false;
        this.loadDavlenie();
      }
    });
  }
}