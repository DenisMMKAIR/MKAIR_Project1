import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VerificationsClient, ServiceResult, FileParameter, VerificationGroup } from '../../api-client';
import { AddValuesService } from '../../services/add-values.service';

@Component({
  selector: 'app-add-values',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-values.page.html',
  styleUrls: ['./add-values.page.scss'],
  providers: [VerificationsClient],
})
export class AddValuesPage implements OnInit {
  private readonly api = inject(VerificationsClient);
  private readonly addValuesService = inject(AddValuesService);

  public excelFile: File | null = null;
  public loading: boolean = false;
  public error: string | null = null;
  public result: ServiceResult | null = null;

  public readonly verificationGroups = Object.values(VerificationGroup);

  ngOnInit(): void {
    this.loadFormData();
  }

  private loadFormData(): void {
    this.mode = this.addValuesService.getMode();
    this.sheetName = this.addValuesService.getSheetName();
    this.dataRange = this.addValuesService.getDataRange();
    this.location = this.addValuesService.getLocation();
    this.group = this.addValuesService.getGroup();
    this.worker = this.addValuesService.getWorker();
    this.pressure = this.addValuesService.getPressure();
    this.temperature = this.addValuesService.getTemperature();
    this.humidity = this.addValuesService.getHumidity();
    this.measurementRange = this.addValuesService.getMeasurementRange();
    this.accuracy = this.addValuesService.getAccuracy();
  }

  public get mode(): 'setValues' | 'setVerificationNum' {
    return this.addValuesService.getMode();
  }

  public set mode(value: 'setValues' | 'setVerificationNum') {
    this.addValuesService.setMode(value);
  }

  public get sheetName(): string {
    return this.addValuesService.getSheetName();
  }

  public set sheetName(value: string) {
    this.addValuesService.setSheetName(value);
  }

  public get dataRange(): string {
    return this.addValuesService.getDataRange();
  }

  public set dataRange(value: string) {
    this.addValuesService.setDataRange(value);
  }

  public get location(): string {
    return this.addValuesService.getLocation();
  }

  public set location(value: string) {
    this.addValuesService.setLocation(value);
  }

  public get group(): string {
    return this.addValuesService.getGroup();
  }

  public set group(value: string) {
    this.addValuesService.setGroup(value);
  }

  public get worker(): boolean {
    return this.addValuesService.getWorker();
  }

  public set worker(value: boolean) {
    this.addValuesService.setWorker(value);
  }

  public get pressure(): boolean {
    return this.addValuesService.getPressure();
  }

  public set pressure(value: boolean) {
    this.addValuesService.setPressure(value);
  }

  public get temperature(): boolean {
    return this.addValuesService.getTemperature();
  }

  public set temperature(value: boolean) {
    this.addValuesService.setTemperature(value);
  }

  public get humidity(): boolean {
    return this.addValuesService.getHumidity();
  }

  public set humidity(value: boolean) {
    this.addValuesService.setHumidity(value);
  }

  public get measurementRange(): boolean {
    return this.addValuesService.getMeasurementRange();
  }

  public set measurementRange(value: boolean) {
    this.addValuesService.setMeasurementRange(value);
  }

  public get accuracy(): boolean {
    return this.addValuesService.getAccuracy();
  }

  public set accuracy(value: boolean) {
    this.addValuesService.setAccuracy(value);
  }

  public switchMode(newMode: 'setValues' | 'setVerificationNum'): void {
    this.mode = newMode;
    this.clearFileControls();
  }

  public onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.excelFile = input.files[0];
    }
  }

  public submit(): void {
    if (!this.excelFile) {
      this.error = 'Выберите файл Excel.';
      return;
    }

    if (!this.sheetName.trim()) {
      this.error = 'Укажите имя листа.';
      return;
    }

    if (!this.dataRange.trim()) {
      this.error = 'Укажите диапазон данных.';
      return;
    }

    if (this.mode === 'setValues' && !this.location) {
      this.error = 'Укажите расположение.';
      return;
    }

    if (this.mode === 'setValues' && !this.group) {
      this.error = 'Укажите группу поверки.';
      return;
    }

    this.loading = true;
    this.error = null;
    this.result = null;

    const fileParam: FileParameter = { data: this.excelFile, fileName: this.excelFile.name };

    if (this.mode === 'setValues') {
      this.api.setValues(
        fileParam,
        this.sheetName.trim(),
        this.dataRange.trim(),
        this.location,
        this.group || null,
        false,
        this.worker,
        this.pressure,
        this.temperature,
        this.humidity,
        this.measurementRange,
        this.accuracy
      ).subscribe({
        next: (res: any) => {
          this.result = res;
          this.loading = false;
        },
        error: (err: any) => {
          this.error = err?.error?.error || 'Ошибка при отправке данных.';
          this.loading = false;
        }
      });
    } else {
      this.api.setVerificationNum(
        this.sheetName.trim(),
        fileParam,
        this.dataRange.trim()
      ).subscribe({
        next: (res: any) => {
          this.result = res;
          this.loading = false;
        },
        error: (err: any) => {
          this.error = err?.error?.error || 'Ошибка при отправке данных.';
          this.loading = false;
        }
      });
    }
  }

  public resetForm(): void {
    this.addValuesService.resetForm();
    this.loadFormData();
    this.clearFileControls();
  }

  private clearFileControls(): void {
    this.excelFile = null;
    this.loading = false;
    this.error = null;
    this.result = null;
  }
} 