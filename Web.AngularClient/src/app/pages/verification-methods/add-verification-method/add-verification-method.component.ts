import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { VerificationMethodsClient } from '../../../api-client';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-verification-method',
  standalone: true,
  templateUrl: './add-verification-method.component.html',
  styleUrls: ['./add-verification-method.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule, FormsModule],
  providers: [VerificationMethodsClient],
})
export class AddVerificationMethodComponent {
  form: FormGroup;
  loading = false;
  result: string | null = null;
  @Output() added = new EventEmitter<void>();
  file: File | null = null;

  constructor(private fb: FormBuilder, private client: VerificationMethodsClient) {
    this.form = this.fb.group({
      description: ['', Validators.required],
      file: [null],
      aliases: this.fb.array([], Validators.required),
      checkups: this.fb.array([], Validators.required)
    });
  }

  get aliases() {
    return this.form.get('aliases') as FormArray;
  }

  get checkups() {
    return this.form.get('checkups') as FormArray;
  }

  addAlias(input: string) {
    const value = input.trim();
    if (value && !this.aliases.value.includes(value)) {
      this.aliases.push(new FormControl(value));
    }
  }

  removeAlias(index: number) {
    this.aliases.removeAt(index);
  }

  addCheckup(key: string, value: string) {
    const k = key.trim();
    const v = value.trim();
    if (k && v && !this.checkups.value.some((c: any) => c.key === k)) {
      this.checkups.push(this.fb.group({ key: [k], value: [v] }));
    }
  }

  removeCheckup(index: number) {
    this.checkups.removeAt(index);
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length) {
      const file = input.files[0];
      if (file.type !== 'application/pdf') {
        this.result = 'Только PDF файлы разрешены';
        this.form.patchValue({ file: null });
        this.file = null;
        return;
      }
      this.file = file;
      this.form.patchValue({ file });
    }
  }

  private isValid(): boolean {
    if (!this.form.value.description) {
      this.result = 'Заполните все обязательные поля';
      return false;
    }
    if (this.aliases.length === 0) {
      this.result = 'Добавьте хотя бы один псевдоним';
      return false;
    }
    if (this.checkups.length === 0) {
      this.result = 'Добавьте хотя бы одну проверку';
      return false;
    }
    if ((this.file && !this.file.name) || (!this.file && this.form.value.file)) {
      this.result = 'Укажите и файл, и имя файла, или оставьте оба поля пустыми';
      return false;
    }
    return true;
  }

  private resetFormState() {
    this.form.reset();
    this.file = null;
    this.aliases.clear();
    this.checkups.clear();
  }

  submit() {
    if (!this.isValid()) return;
    this.loading = true;
    this.result = null;
    const description = this.form.value.description;
    const aliases = this.aliases.value;
    const checkupsObj: { [key: string]: string } = {};
    this.checkups.value.forEach((c: any) => {
      checkupsObj[c.key] = c.value;
    });
    const fileName = this.file ? this.file.name : null;
    const fileParam = this.file ? { data: this.file, fileName: this.file.name } : null;
    this.client.addVerificationMethod(
      fileName,
      fileParam,
      description,
      aliases,
      checkupsObj
    ).subscribe({
      next: res => {
        if (res && !res.error) {
          this.result = 'Метод успешно добавлен';
          this.resetFormState();
          this.added.emit();
        } else {
          this.result = res?.error || 'Ошибка при добавлении';
        }
        this.loading = false;
      },
      error: () => {
        this.result = 'Ошибка при добавлении';
        this.loading = false;
      }
    });
  }
} 