import { Component, inject, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VerificationsClient, SuccessVerificationDto, DeviceLocation } from '../../api-client';
import { VerificationsService } from '../../services/verifications.service';
import { DatesFilterComponent } from '../../shared/dates-filter/dates-filter.component';

@Component({
  selector: 'app-filled-verifications-page',
  standalone: true,
  templateUrl: './filled-verifications.page.html',
  styleUrls: ['./filled-verifications.page.scss'],
  imports: [NgFor, NgIf, DatePipe, FormsModule, DatesFilterComponent, DecimalPipe],
  providers: [VerificationsClient],
})
export class FilledVerificationsPage implements OnInit {
  private readonly verificationsClient = inject(VerificationsClient);
  private readonly verificationsService = inject(VerificationsService);

  public verifications: SuccessVerificationDto[] = [];
  public loading = false;
  public error: string | null = null;

  ngOnInit(): void {
    this.verificationsService.setPageChangeCallback(() => this.loadVerifications());
    this.loadVerifications();
  }

  public loadVerifications(): void {
    this.loading = true;
    this.error = null;
    const deviceTypeNumberFilter = this.deviceTypeNumberFilter === null || this.deviceTypeNumberFilter === '' || this.deviceTypeNumberFilter === 'all'
      ? null
      : this.deviceTypeNumberFilter;
    const yearMonthFilter = this.yearMonthFilter === null || this.yearMonthFilter === 'all'
      ? null 
      : this.yearMonthFilter;
    const typeInfoFilter = this.typeInfoFilter === null || this.typeInfoFilter === '' || this.typeInfoFilter === 'all'
      ? null 
      : this.typeInfoFilter;
    const locationFilter = this.locationFilter === null || this.locationFilter === 'all'
      ? null 
      : this.locationFilter as DeviceLocation;
    this.verificationsClient.getVerifications(
      this.pagination.currentPage, 
      this.pagination.pageSize, 
      deviceTypeNumberFilter,
      yearMonthFilter,
      typeInfoFilter,
      locationFilter
    ).subscribe({
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

  public onLocationFilterChange(): void {
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

  public get typeInfoFilter() {
    return this.verificationsService.getTypeInfoFilter();
  }

  public set typeInfoFilter(value: string | null) {
    this.verificationsService.setTypeInfoFilter(value);
  }

  public get locationFilter() {
    return this.verificationsService.getLocationFilter();
  }

  public set locationFilter(value: string | null) {
    this.verificationsService.setLocationFilter(value);
  }

  public get deviceTypeNumberFilter() {
    return this.verificationsService.getDeviceTypeNumberFilter();
  }

  public set deviceTypeNumberFilter(value: string | null) {
    this.verificationsService.setDeviceTypeNumberFilter(value);
  }
} 