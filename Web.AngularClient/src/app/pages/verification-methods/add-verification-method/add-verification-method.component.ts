import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
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
  aliases: string[] = [];
  aliasInput: string = '';

  constructor(private fb: FormBuilder, private client: VerificationMethodsClient) {
    this.form = this.fb.group({
      description: ['', Validators.required],
      file: [null, Validators.required]
    });
  }

  addAlias() {
    const value = this.aliasInput.trim();
    if (value && !this.aliases.includes(value)) {
      this.aliases.push(value);
      this.aliasInput = '';
    }
  }

  removeAlias(alias: string) {
    this.aliases = this.aliases.filter(a => a !== alias);
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

  submit() {
    if (this.form.invalid || !this.file) return;
    this.loading = true;
    this.result = null;
    const description = this.form.value.description;
    const aliases = this.aliases.length ? this.aliases : null;
    const fileName = this.file.name;
    const fileParam = { data: this.file, fileName: this.file.name };
    this.client.addVerificationMethod(description, aliases, fileName, fileParam).subscribe({
      next: res => {
        if (res && !res.error) {
          this.result = 'Метод успешно добавлен';
          this.form.reset();
          this.file = null;
          this.aliases = [];
          this.aliasInput = '';
          this.added.emit();
        } else {
          this.result = res?.error || 'Ошибка при добавлении';
        }
        this.loading = false;
      },
      error: err => {
        this.result = 'Ошибка при добавлении';
        this.loading = false;
      }
    });
  }
} 