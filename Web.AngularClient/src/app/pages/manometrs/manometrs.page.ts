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
  public error: string | null = null;

  ngOnInit(): void {
    this.manometrsService.setPageChangeCallback(() => this.loadManometrs());
    this.loadManometrs();
  }

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

  public get ownerFilter() {
    return this.manometrsService.getOwnerFilter();
  }

  public set ownerFilter(value: string | null) {
    this.manometrsService.setOwnerFilter(value);
  }

  public formatRangeAccuracy(m: Manometr1VerificationDto): string {
    if (m.measurementMin != null && m.measurementMax != null && m.measurementUnit) {
      let range = `${m.measurementMin}–${m.measurementMax} ${m.measurementUnit}`;
      if (m.validError != null) {
        range += `; ±${m.validError}`;
      }
      return range;
    }
    if (m.validError != null) {
      return `±${m.validError}`;
    }
    return '-';
  }

  public formatDeviceValues(m: Manometr1VerificationDto): string {
    if (m.deviceValues && m.deviceValues.length > 0) {
      return m.deviceValues.map(row => row.join(', ')).join('; ');
    }
    return '-';
  }

  public formatEtalonValues(m: Manometr1VerificationDto): string {
    if (m.etalonValues && m.etalonValues.length > 0) {
      return m.etalonValues.map(row => row.join(', ')).join('; ');
    }
    return '-';
  }

  public formatActualError(m: Manometr1VerificationDto): string {
    if (m.actualError && m.actualError.length > 0) {
      return m.actualError.map(row => row.join(', ')).join('; ');
    }
    return '-';
  }

  public formatActualVariation(m: Manometr1VerificationDto): string {
    if (m.actualVariation && m.actualVariation.length > 0) {
      return m.actualVariation.join(', ');
    }
    return '-';
  }
} 