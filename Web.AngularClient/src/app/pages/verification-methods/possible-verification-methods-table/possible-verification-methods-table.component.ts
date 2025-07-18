import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  VerificationMethodsClient,
  PossibleVrfMethodDTO,
  YearMonth,
  ShowVMethods,
} from '../../../api-client';
import { VerificationMethodsService } from '../../../services/verification-methods.service';
import { DatesFilterComponent } from '../../../shared/dates-filter/dates-filter.component';
import { Subject, takeUntil, switchMap } from 'rxjs';

@Component({
  selector: 'app-possible-verification-methods-table',
  standalone: true,
  templateUrl: './possible-verification-methods-table.component.html',
  styleUrls: ['./possible-verification-methods-table.component.scss'],
  imports: [NgFor, NgIf, FormsModule, DatesFilterComponent],
  providers: [VerificationMethodsClient],
})
export class PossibleVerificationMethodsTableComponent implements OnInit, OnDestroy {
  private readonly verificationMethodsClient = inject(
    VerificationMethodsClient
  );
  private readonly verificationMethodsService = inject(
    VerificationMethodsService
  );
  private readonly key = 'possible' as const;
  private readonly destroy$ = new Subject<void>();
  private readonly request$ = new Subject<void>();

  public possibleVerificationMethods: PossibleVrfMethodDTO[] = [];
  public loading = false;
  public error: string | null = null;
  public aliasErrorMessage: string | null = null;
  public showAllTypeNumbers = false;
  public showVMethods: ShowVMethods = ShowVMethods.Частичные;

  ngOnInit(): void {
    this.verificationMethodsService.setPageChangeCallback(this.key, () =>
      this.loadPossibleVerificationMethods()
    );
    this.request$
      .pipe(
        takeUntil(this.destroy$),
        switchMap(() => {
          this.loading = true;
          this.error = null;
          const yearMonth = !this.yearMonthFilter ? null : this.yearMonthFilter;
          return this.verificationMethodsClient.getPossibleVerificationMethods(
            this.pagination.currentPage,
            this.pagination.pageSize,
            this.showVMethods,
            this.deviceTypeNumberFilter || null,
            this.verificationNameFilter,
            this.deviceTypeInfoFilter,
            yearMonth
          );
        })
      )
      .subscribe({
        next: (result) => {
          if (result.data) {
            this.possibleVerificationMethods = result.data.items ?? [];
            this.verificationMethodsService.updatePaginationFromData(
              this.key,
              result.data
            );
          } else {
            this.possibleVerificationMethods = [];
            this.verificationMethodsService.resetPagination(this.key);
          }
          this.loading = false;
        },
        error: (err) => {
          const msg = err?.error?.error || err?.error?.message || err?.message;
          this.error = msg || 'Не удалось загрузить возможные методы поверки';
          this.loading = false;
        },
      });
    this.loadPossibleVerificationMethods();
  }

  public get verificationNameFilter() {
    return this.verificationMethodsService.getVerificationNameFilter(this.key);
  }
  public set verificationNameFilter(value: string | null) {
    this.verificationMethodsService.setVerificationNameFilter(this.key, value);
  }
  public get deviceTypeInfoFilter() {
    return this.verificationMethodsService.getDeviceTypeInfoFilter(this.key);
  }
  public set deviceTypeInfoFilter(value: string | null) {
    this.verificationMethodsService.setDeviceTypeInfoFilter(this.key, value);
  }
  public get yearMonthFilter() {
    return this.verificationMethodsService.getYearMonthFilter(this.key);
  }
  public set yearMonthFilter(value: string | null) {
    this.verificationMethodsService.setYearMonthFilter(this.key, value);
  }
  public get deviceTypeNumberFilter() {
    return this.verificationMethodsService.getDeviceTypeNumberFilter(this.key);
  }
  public set deviceTypeNumberFilter(value: string) {
    this.verificationMethodsService.setDeviceTypeNumberFilter(this.key, value);
    this.verificationMethodsService.resetToFirstPage(this.key);
    this.loadPossibleVerificationMethods();
  }
  public onFilterChange(): void {
    this.verificationMethodsService.resetToFirstPage(this.key);
    this.loadPossibleVerificationMethods();
  }

  public onShowVMethodsChange(): void {
    this.verificationMethodsService.resetToFirstPage(this.key);
    this.loadPossibleVerificationMethods();
  }

  public loadPossibleVerificationMethods(): void {
    this.request$.next();
  }

  public get pagination() {
    return this.verificationMethodsService.getPagination(this.key);
  }

  public formatDates(dates?: YearMonth[]): string {
    if (!dates || dates.length === 0) return '-';
    return dates
      .map(
        (d) =>
          `${d.year ?? 0}.${
            (d.month ?? 0) < 10 ? '0' + (d.month ?? 0) : d.month ?? 0
          }`
      )
      .join(', ');
  }

  public formatAliases(aliases?: { alias?: string }[]): string {
    if (!aliases || aliases.length === 0) return '-';
    return aliases
      .map((a) => a?.alias)
      .filter(Boolean)
      .join(', ');
  }

  public reload() {
    this.loadPossibleVerificationMethods();
  }

  public getDeviceModifications(m: PossibleVrfMethodDTO): string[] {
    if (!m.aliasGroups || m.aliasGroups.length === 0) return [];
    return m.aliasGroups.flatMap(g => g.modifications || []);
  }

  public getAliases(m: PossibleVrfMethodDTO): { exists?: boolean; alias?: string }[] {
    if (!m.aliasGroups || m.aliasGroups.length === 0) return [];
    return m.aliasGroups.flatMap(g => g.aliases || []);
  }

  public getDates(m: PossibleVrfMethodDTO): YearMonth[] {
    if (!m.aliasGroups || m.aliasGroups.length === 0) return [];
    return m.aliasGroups.flatMap(g => g.dates || []);
  }

  public getAllDocs(m: PossibleVrfMethodDTO): {url: string, label: string}[] {
    const docs: {url: string, label: string}[] = [];
    let methodCount = 1;
    let specCount = 1;
    if (m.methodUrls) {
      for (const url of m.methodUrls) {
        docs.push({ url, label: `Методика${methodCount++}` });
      }
    }
    if (m.specUrls) {
      for (const url of m.specUrls) {
        docs.push({ url, label: `Спецификация${specCount++}` });
      }
    }
    return docs;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
