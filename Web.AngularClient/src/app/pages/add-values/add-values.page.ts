import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VerificationsClient, ServiceResult, FileParameter } from '../../api-client';

@Component({
  selector: 'app-add-values',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-values.page.html',
  styleUrls: ['./add-values.page.scss'],
  providers: [VerificationsClient],
})
export class AddValuesPage {
  private readonly api = inject(VerificationsClient);

  public mode: 'setValues' | 'setVerificationNum' = 'setValues';

  public excelFile: File | null = null;
  public sheetName: string = '';
  public dataRange: string = '';
  public location: string = '';
  public worker: boolean = false;
  public additionalInfo: boolean = false;
  public pressure: boolean = false;
  public temperature: boolean = false;
  public humidity: boolean = false;
  public measurementRange: boolean = false;
  public accuracy: boolean = false;
  public loading = false;
  public error: string | null = null;
  public result: ServiceResult | null = null;

  public excelFileNum: File | null = null;
  public sheetNameNum: string = '';
  public dataRangeNum: string = '';
  public loadingNum = false;
  public errorNum: string | null = null;
  public resultNum: ServiceResult | null = null;

  public switchMode(newMode: 'setValues' | 'setVerificationNum'): void {
    this.mode = newMode;
    this.clearControls();
  }

  public clearControls(): void {
    this.excelFile = null;
    this.sheetName = '';
    this.dataRange = '';
    this.location = '';
    this.worker = false;
    this.additionalInfo = false;
    this.pressure = false;
    this.temperature = false;
    this.humidity = false;
    this.measurementRange = false;
    this.accuracy = false;
    this.loading = false;
    this.error = null;
    this.result = null;
    this.loadingNum = false;
    this.errorNum = null;
    this.resultNum = null;
  }

  public onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.excelFile = input.files[0];
    }
  }

  public onFileChangeNum(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.excelFileNum = input.files[0];
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
        false, // verificationTypeNum
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
} 