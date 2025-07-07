import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InitialVerificationsClient, FileParameter, ServiceResult, DeviceLocation } from '../api-client';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-add-values',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-values.page.html',
  styleUrls: ['./add-values.page.scss']
})
export class AddValuesPage {
  private readonly api: InitialVerificationsClient;

  public excelFile: File | null = null;
  public sheetName: string = '';
  public dataRange: string = '';
  public location: DeviceLocation = DeviceLocation.АнтипинскийНПЗ;
  public verificationTypeNum: boolean = false;
  public worker: boolean = false;
  public additionalInfo: boolean = false;
  public pressure: boolean = false;
  public temperature: boolean = false;
  public humidity: boolean = false;

  public loading = false;
  public error: string | null = null;
  public result: ServiceResult | null = null;

  constructor(http: HttpClient) {
    this.api = new InitialVerificationsClient(http);
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
    this.loading = true;
    this.error = null;
    this.result = null;
    const fileParam: FileParameter = { data: this.excelFile, fileName: this.excelFile.name };
    this.api.setValues(
      fileParam,
      this.sheetName,
      this.dataRange,
      this.location,
      this.verificationTypeNum,
      this.worker,
      this.additionalInfo,
      this.pressure,
      this.temperature,
      this.humidity
    ).subscribe({
      next: (res) => {
        this.result = res;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.error || 'Ошибка при отправке данных.';
        this.loading = false;
      }
    });
  }
} 