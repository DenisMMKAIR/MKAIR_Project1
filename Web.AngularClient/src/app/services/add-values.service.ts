import { Injectable } from '@angular/core';

export interface IAddValuesFormData {
  mode: 'setValues' | 'setVerificationNum';
  sheetName: string;
  dataRange: string;
  location: string;
  group: string;
  worker: boolean;
  pressure: boolean;
  temperature: boolean;
  humidity: boolean;
  measurementRange: boolean;
  accuracy: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AddValuesService {
  private currentFormData: IAddValuesFormData = {
    mode: 'setValues',
    sheetName: '',
    dataRange: '',
    location: '',
    group: '',
    worker: false,
    pressure: false,
    temperature: false,
    humidity: false,
    measurementRange: false,
    accuracy: false
  };

  public getMode(): 'setValues' | 'setVerificationNum' {
    return this.currentFormData.mode;
  }

  public setMode(mode: 'setValues' | 'setVerificationNum'): void {
    this.currentFormData.mode = mode;
  }

  public getSheetName(): string {
    return this.currentFormData.sheetName;
  }

  public setSheetName(sheetName: string): void {
    this.currentFormData.sheetName = sheetName;
  }

  public getDataRange(): string {
    return this.currentFormData.dataRange;
  }

  public setDataRange(dataRange: string): void {
    this.currentFormData.dataRange = dataRange;
  }

  public getLocation(): string {
    return this.currentFormData.location;
  }

  public setLocation(location: string): void {
    this.currentFormData.location = location;
  }

  public getGroup(): string {
    return this.currentFormData.group;
  }

  public setGroup(group: string): void {
    this.currentFormData.group = group;
  }

  public getWorker(): boolean {
    return this.currentFormData.worker;
  }

  public setWorker(worker: boolean): void {
    this.currentFormData.worker = worker;
  }

  public getPressure(): boolean {
    return this.currentFormData.pressure;
  }

  public setPressure(pressure: boolean): void {
    this.currentFormData.pressure = pressure;
  }

  public getTemperature(): boolean {
    return this.currentFormData.temperature;
  }

  public setTemperature(temperature: boolean): void {
    this.currentFormData.temperature = temperature;
  }

  public getHumidity(): boolean {
    return this.currentFormData.humidity;
  }

  public setHumidity(humidity: boolean): void {
    this.currentFormData.humidity = humidity;
  }

  public getMeasurementRange(): boolean {
    return this.currentFormData.measurementRange;
  }

  public setMeasurementRange(measurementRange: boolean): void {
    this.currentFormData.measurementRange = measurementRange;
  }

  public getAccuracy(): boolean {
    return this.currentFormData.accuracy;
  }

  public setAccuracy(accuracy: boolean): void {
    this.currentFormData.accuracy = accuracy;
  }

  public resetForm(): void {
    this.currentFormData = {
      mode: 'setValues',
      sheetName: '',
      dataRange: '',
      location: '',
      group: '',
      worker: false,
      pressure: false,
      temperature: false,
      humidity: false,
      measurementRange: false,
      accuracy: false
    };
  }
} 