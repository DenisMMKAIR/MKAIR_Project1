import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManometrClient, Manometr1VerificationDto } from '../../api-client';
import { ManometrsService } from '../../services/manometrs.service';
import { DatesFilterComponent } from '../../shared/dates-filter/dates-filter.component';

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
  public selectedRows = new Set<number>();

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

  // Data loading methods
  public loadManometrs(): void {
    this.loading = true;
    this.error = null;
    
    this.manometrClient.getVerifications(
      this.pagination.currentPage, 
      this.pagination.pageSize
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

  public exportToPdf(): void {
    if (this.selectedRows.size === 0 || this.exportLoading) {
      return;
    }
    
    this.exportLoading = true;
    const selectedManometrs = Array.from(this.selectedRows).map(index => this.manometrs[index]);
    
    console.log('Export to PDF clicked', selectedManometrs);
    // TODO: Implement PDF export functionality with selected items
    // This would typically call the backend API to export the selected data
    
    // Simulate API call - replace with actual API call
    setTimeout(() => {
      this.exportLoading = false;
      // Clear selected rows on successful export
      this.selectedRows.clear();
      // Handle success/error here
    }, 2000);
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
}