import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AddVerificationMethodComponent } from './add-verification-method.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { VerificationMethodsClient, ServiceResult, AddVerificationMethodRequest } from '../../../api-client';
import { VerificationMethodsService } from '../../../services/verification-methods.service';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';

const TEST_DATA = {
  DESCRIPTION: 'Test description',
  ALIAS: 'alias1',
  CHECKUP_KEY: 'visual',
  CHECKUP_VALUE: '5.1',
  CHECKUP_KEY_2: 'result',
  CHECKUP_VALUE_2: '5.2.3',
  CHECKUP_KEY_3: 'accuracy',
  CHECKUP_VALUE_3: '5.3'
} as const;

const MOCK_SERVICE_RESULT = new ServiceResult();
MOCK_SERVICE_RESULT.message = 'Метод поверки добавлен. Присвоен устройствам 57. Добавлены псевдонимов 3';
MOCK_SERVICE_RESULT.error = undefined;

describe('AddVerificationMethodComponent', () => {
  let component: AddVerificationMethodComponent;
  let fixture: ComponentFixture<AddVerificationMethodComponent>;
  let mockVerificationMethodsClient: jasmine.SpyObj<VerificationMethodsClient>;
  let mockVerificationMethodsService: jasmine.SpyObj<VerificationMethodsService>;

  beforeEach(async () => {
    mockVerificationMethodsClient = jasmine.createSpyObj('VerificationMethodsClient', ['addVerificationMethod']);
    mockVerificationMethodsService = jasmine.createSpyObj('VerificationMethodsService', [
      'getAddVerificationMethodFormData',
      'setAddVerificationMethodFormData',
      'clearAddVerificationMethodFormData'
    ]);

    mockVerificationMethodsService.getAddVerificationMethodFormData.and.returnValue({
      description: '',
      aliasInput: '',
      checkupKey: '',
      checkupValue: ''
    });

    await TestBed.configureTestingModule({
      imports: [AddVerificationMethodComponent, ReactiveFormsModule, HttpClientTestingModule],
      providers: [
        FormBuilder,
        { provide: VerificationMethodsClient, useValue: mockVerificationMethodsClient },
        { provide: VerificationMethodsService, useValue: mockVerificationMethodsService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AddVerificationMethodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  describe('Component Initialization', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize form with saved data', () => {
      expect(mockVerificationMethodsService.getAddVerificationMethodFormData).toHaveBeenCalled();
    });
  });

  describe('Form Validation', () => {
    it('should not submit if form is invalid', fakeAsync(() => {
      component.form.markAllAsTouched();
      component.form.updateValueAndValidity();
      fixture.detectChanges();
      
      component.submit();
      
      expect(mockVerificationMethodsClient.addVerificationMethod).not.toHaveBeenCalled();
      expect(component.result).toContain('Заполните описание');
    }));

    it('should show validation error for missing aliases', () => {
      component.form.get('description')?.setValue('desc');
      component.form.markAllAsTouched();
      component.form.updateValueAndValidity();
      fixture.detectChanges();
      
      component.submit();
      
      expect(component.result).toContain('Добавьте хотя бы один псевдоним');
    });

    it('should show validation error for missing checkups', () => {
      component.form.get('description')?.setValue('desc');
      component.form.get('aliasInput')?.setValue(TEST_DATA.ALIAS);
      component.addAlias();
      component.form.markAllAsTouched();
      component.form.updateValueAndValidity();
      fixture.detectChanges();
      
      component.submit();
      
      expect(component.result).toContain('Не указаны пункты проверки');
    });
  });

  describe('Form Submission', () => {
    beforeEach(() => {
      fillValidForm();
    });

    it('should submit valid form and show success', fakeAsync(() => {
      mockVerificationMethodsClient.addVerificationMethod.and.returnValue(of(MOCK_SERVICE_RESULT));
      spyOn(component.added, 'emit');
      
      component.submit();
      tick();
      
      expect(mockVerificationMethodsClient.addVerificationMethod).toHaveBeenCalled();
      expect(component.result).toBe('Метод успешно добавлен');
      expect(component.added.emit).toHaveBeenCalled();
      expect(mockVerificationMethodsService.clearAddVerificationMethodFormData).toHaveBeenCalled();
    }));

    it('should show error on API failure', fakeAsync(() => {
      mockVerificationMethodsClient.addVerificationMethod.and.returnValue(throwError(() => new Error('API Error')));
      
      component.submit();
      tick();
      
      expect(component.result).toBe('Ошибка при добавлении');
    }));

    it('should send checkups in correct format', fakeAsync(() => {
      mockVerificationMethodsClient.addVerificationMethod.and.returnValue(of(MOCK_SERVICE_RESULT));
      
      component.submit();
      tick();
      
      if (mockVerificationMethodsClient.addVerificationMethod.calls.any()) {
        const callArgs = mockVerificationMethodsClient.addVerificationMethod.calls.mostRecent().args;
        const request = callArgs[0] as AddVerificationMethodRequest;
        expect(request.checkups).toEqual({ 
          visual: '5.1', 
          result: '5.2.3', 
          accuracy: '5.3' 
        } as any);
      } else {
        fail('addVerificationMethod was not called');
      }
    }));
  });

  describe('Alias Management', () => {
    it('should add alias when valid', () => {
      component.form.get('aliasInput')?.setValue(TEST_DATA.ALIAS);
      
      component.addAlias();
      
      expect(component.aliases.length).toBe(1);
      expect(component.aliases.at(0).value).toBe(TEST_DATA.ALIAS);
    });

    it('should not add duplicate aliases', () => {
      component.form.get('aliasInput')?.setValue(TEST_DATA.ALIAS);
      component.addAlias();
      component.form.get('aliasInput')?.setValue(TEST_DATA.ALIAS);
      component.addAlias();
      
      expect(component.aliases.length).toBe(1);
    });

    it('should remove alias', () => {
      component.form.get('aliasInput')?.setValue(TEST_DATA.ALIAS);
      component.addAlias();
      
      component.removeAlias(0);
      
      expect(component.aliases.length).toBe(0);
    });

    it('should reset alias input after adding', () => {
      component.form.get('aliasInput')?.setValue(TEST_DATA.ALIAS);
      
      component.addAlias();
      
      expect(component.aliasInputValue).toBe('');
    });
  });

  describe('Checkup Management', () => {
    it('should add checkup when valid', () => {
      component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY);
      component.form.get('checkupValue')?.setValue(TEST_DATA.CHECKUP_VALUE);
      
      component.addCheckup();
      
      expect(component.checkups.length).toBe(1);
      expect(component.checkups.at(0).value).toEqual({
        key: TEST_DATA.CHECKUP_KEY,
        value: TEST_DATA.CHECKUP_VALUE
      });
    });

    it('should not add duplicate checkup keys', () => {
      component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY);
      component.form.get('checkupValue')?.setValue(TEST_DATA.CHECKUP_VALUE);
      component.addCheckup();
      component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY);
      component.form.get('checkupValue')?.setValue('another');
      component.addCheckup();
      
      expect(component.checkups.length).toBe(1);
    });

    it('should remove checkup', () => {
      component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY);
      component.form.get('checkupValue')?.setValue(TEST_DATA.CHECKUP_VALUE);
      component.addCheckup();
      
      component.removeCheckup(0);
      
      expect(component.checkups.length).toBe(0);
    });

    it('should reset checkup inputs after adding', () => {
      component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY);
      component.form.get('checkupValue')?.setValue(TEST_DATA.CHECKUP_VALUE);
      
      component.addCheckup();
      
      expect(component.checkupKeyValue).toBe('');
      expect(component.checkupValueValue).toBe('');
    });
  });

  describe('File Handling', () => {
    it('should handle valid file upload', () => {
      const file = new File(['test'], 'test.pdf', { type: 'application/pdf' });
      
      component.onFileChange({ target: { files: [file] } } as any);
      
      expect(component.file).toBe(file);
    });

    it('should handle file removal', () => {
      const file = new File(['test'], 'test.pdf', { type: 'application/pdf' });
      component.onFileChange({ target: { files: [file] } } as any);
      
      component.removeFile();
      
      expect(component.file).toBeNull();
    });

    it('should reject oversized files', () => {
      const largeFile = new File(['x'.repeat(11 * 1024 * 1024)], 'large.pdf', { type: 'application/pdf' });
      
      component.onFileChange({ target: { files: [largeFile] } } as any);
      
      expect(component.file).toBeNull();
      expect(component.result).toContain('Только PDF, DOC, DOCX, XLS, XLSX файлы до 10 МБ разрешены');
    });

    it('should reject invalid file types', () => {
      const invalidFile = new File(['test'], 'test.txt', { type: 'text/plain' });
      
      component.onFileChange({ target: { files: [invalidFile] } } as any);
      
      expect(component.file).toBeNull();
      expect(component.result).toContain('Только PDF, DOC, DOCX, XLS, XLSX файлы до 10 МБ разрешены');
    });
  });

  describe('Form Persistence', () => {
    it('should save form data on changes', () => {
      component.form.get('description')?.setValue(TEST_DATA.DESCRIPTION);
      
      expect(mockVerificationMethodsService.setAddVerificationMethodFormData).toHaveBeenCalledWith(
        jasmine.objectContaining({
          description: TEST_DATA.DESCRIPTION
        })
      );
    });
  });

  // Helper function
  function fillValidForm() {
    component.form.get('description')?.setValue(TEST_DATA.DESCRIPTION);
    component.form.get('aliasInput')?.setValue(TEST_DATA.ALIAS);
    component.addAlias();
    fixture.detectChanges();
    
    component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY);
    component.form.get('checkupValue')?.setValue(TEST_DATA.CHECKUP_VALUE);
    component.addCheckup();
    fixture.detectChanges();
    
    component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY_2);
    component.form.get('checkupValue')?.setValue(TEST_DATA.CHECKUP_VALUE_2);
    component.addCheckup();
    fixture.detectChanges();
    
    component.form.get('checkupKey')?.setValue(TEST_DATA.CHECKUP_KEY_3);
    component.form.get('checkupValue')?.setValue(TEST_DATA.CHECKUP_VALUE_3);
    component.addCheckup();
    fixture.detectChanges();
    
    // Ensure form is valid before submission
    expect(component.isFormValid).toBe(true);
  }
}); 