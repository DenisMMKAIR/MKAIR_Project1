import { Component, Output, EventEmitter, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormArray,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import {
  VerificationMethodsClient,
  AddVerificationMethodRequest,
  ChType,
  CheckupType,
} from '../../../api-client';
import { VerificationMethodsService } from '../../../services/verification-methods.service';

const VALIDATION_MESSAGES = {
  DESCRIPTION_REQUIRED: 'Заполните описание',
  ALIASES_REQUIRED: 'Добавьте хотя бы один псевдоним',
  CHECKUPS_REQUIRED: 'Не указаны пункты проверки',
  FORM_INVALID: 'Проверьте корректность заполнения формы',
  SUCCESS: 'Метод успешно добавлен',
  ERROR: 'Ошибка при добавлении',
} as const;

const FILE_CONSTRAINTS = {
  MAX_SIZE: 10 * 1024 * 1024, // 10MB
  ALLOWED_EXTENSIONS: /\.(pdf|docx?|xlsx?)$/i,
} as const;

@Component({
  selector: 'app-add-verification-method',
  standalone: true,
  templateUrl: './add-verification-method.component.html',
  styleUrls: ['./add-verification-method.component.scss'],
  imports: [CommonModule, ReactiveFormsModule],
  providers: [VerificationMethodsClient],
})
export class AddVerificationMethodComponent implements OnDestroy {
  @Output() added = new EventEmitter<void>();
  
  form!: FormGroup;
  loading = false;
  result: string | null = null;
  file: File | null = null;
  chTypeOptions = Object.values(ChType)

  private readonly destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private client: VerificationMethodsClient,
    private verificationMethodsService: VerificationMethodsService
  ) {
    this.initializeForm();
  }

  // Form getters
  get aliases(): FormArray {
    return this.form.get('aliases') as FormArray;
  }

  get checkups(): FormArray {
    return this.form.get('checkups') as FormArray;
  }

  get aliasInputValue(): string {
    return this.form.get('aliasInput')?.value || '';
  }

  get checkupKeyValue(): string {
    return this.form.get('checkupKey')?.value || '';
  }

  get checkupTypeValue(): ChType {
    return this.form.get('checkupType')!.value;
  }

  get checkupValueValue(): string {
    return this.form.get('checkupValue')?.value || '';
  }

  get isFormValid(): boolean {
    return (
      this.form.valid &&
      this.aliases.length > 0 &&
      this.checkups.length > 0 &&
      this.form.get('description')?.value?.trim()
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    const savedData =
      this.verificationMethodsService.getAddVerificationMethodFormData();
    this.form = this.fb.group({
      description: [
        savedData.description,
        [Validators.required, Validators.minLength(3)],
      ],
      aliasInput: [savedData.aliasInput],
      aliases: this.fb.array([]),
      checkupKey: [savedData.checkupKey],
      checkupType: [savedData.checkupType],
      checkupValue: [savedData.checkupValue],
      checkups: this.fb.array([]),
      file: [null],
    });

    this.setupFormPersistence();
  }

  private setupFormPersistence(): void {
    this.form.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((values) => {
        this.verificationMethodsService.setAddVerificationMethodFormData({
          description: values.description || '',
          aliasInput: values.aliasInput || '',
          checkupKey: values.checkupKey || '',
          checkupValue: values.checkupValue || '',
        });
      });
  }

  // Alias management
  addAlias(): void {
    const value = this.aliasInputValue.trim();
    if (this.canAddAlias(value)) {
      this.aliases.push(this.fb.control(value));
      this.resetAliasInput();
    }
  }

  removeAlias(index: number): void {
    this.aliases.removeAt(index);
  }

  private canAddAlias(value: string): boolean {
    return Boolean(value && !this.aliases.value.includes(value));
  }

  private resetAliasInput(): void {
    this.form.get('aliasInput')?.reset();
  }

  addCheckup(): void {
    const key = this.checkupKeyValue.trim();
    const type = this.checkupTypeValue;
    const value = this.checkupValueValue.trim();

    if (this.canAddCheckup(key, value)) {
      this.checkups.push(
        this.fb.group({
          key: [key],
          value: { type: [type], value: [value] },
        })
      );
      this.resetCheckupInputs();
    }
  }

  removeCheckup(index: number): void {
    this.checkups.removeAt(index);
  }

  private canAddCheckup(key: string, value: string): boolean {
    return Boolean(
      key && value && !this.checkups.value.some((c: any) => c.key === key)
    );
  }

  private resetCheckupInputs(): void {
    this.form.get('checkupKey')?.reset();
    this.form.get('checkupValue')?.reset();
  }

  // File handling
  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      if (this.isValidFile(file)) {
        this.file = file;
      } else {
        this.handleFileError();
      }
    }
  }

  removeFile(): void {
    this.file = null;
    this.form.patchValue({ file: null });
  }

  private isValidFile(file: File): boolean {
    return (
      file.size <= FILE_CONSTRAINTS.MAX_SIZE &&
      Boolean(file.name.match(FILE_CONSTRAINTS.ALLOWED_EXTENSIONS))
    );
  }

  private handleFileError(): void {
    this.setResult(
      'Только PDF, DOC, DOCX, XLS, XLSX файлы до 10 МБ разрешены',
      false
    );
    this.form.patchValue({ file: null });
    this.file = null;
  }

  // Form submission
  submit(): void {
    this.validateForm();

    if (!this.isFormValid) {
      this.setResult(this.getValidationError(), false);
      return;
    }

    this.performSubmission();
  }

  private validateForm(): void {
    this.form.markAllAsTouched();
    this.form.updateValueAndValidity();
  }

  private performSubmission(): void {
    this.loading = true;
    this.clearResult();

    const request = this.buildRequest();

    this.client
      .addVerificationMethod(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => this.handleSubmissionSuccess(res),
        error: () => this.handleSubmissionError(),
      });
  }

  private buildRequest(): AddVerificationMethodRequest {
    const request = new AddVerificationMethodRequest();
    request.description = this.form.value.description.trim();
    request.aliases = this.aliases.value;
    const checkupsObj: { [key: string]: CheckupType } = {};

    this.checkups.value.forEach((c: any) => {
      const type: ChType = c.value.type[0];
      const value: string = c.value.value[0];

      checkupsObj[c.key] = new CheckupType({
        type,
        value,
      });
    });

    request.checkups = checkupsObj;

    return request;
  }

  private handleSubmissionSuccess(res: any): void {
    this.loading = false;
    if (res && res.error === null && res.message) {
      this.setResult(VALIDATION_MESSAGES.SUCCESS, true);
      this.added.emit();
      this.resetForm();
    } else {
      this.setResult(res?.error || VALIDATION_MESSAGES.ERROR, false);
    }
  }

  private handleSubmissionError(): void {
    this.loading = false;
    this.setResult(VALIDATION_MESSAGES.ERROR, false);
  }

  private resetForm(): void {
    this.form.reset();
    this.aliases.clear();
    this.checkups.clear();
    this.file = null;
    this.verificationMethodsService.clearAddVerificationMethodFormData();
  }

  // Result management
  private setResult(message: string, success: boolean): void {
    this.result = message;
    if (success) {
      setTimeout(() => (this.result = null), 3000);
    }
  }

  private clearResult(): void {
    this.result = null;
  }

  // Validation
  private getValidationError(): string {
    if (!this.form.value.description?.trim()) {
      return VALIDATION_MESSAGES.DESCRIPTION_REQUIRED;
    }
    if (this.aliases.length === 0) {
      return VALIDATION_MESSAGES.ALIASES_REQUIRED;
    }
    if (this.checkups.length === 0) {
      return VALIDATION_MESSAGES.CHECKUPS_REQUIRED;
    }
    return VALIDATION_MESSAGES.FORM_INVALID;
  }
}
