import {Component, output, inject, OnInit} from '@angular/core';
import {
    FormBuilder,
    FormGroup,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
/* import { ToastrService } from 'ngx-toastr';
 */import {NgIf} from '@angular/common';
import {passwordValidator} from '../_validators/password.validator';
import {usernameValidator} from '../_validators/username.validator';
import {confirmPasswordValidator} from '../_validators/confrim-password.validator';
import {TextInputComponent} from '../_forms/text-input/text-input.component';
import {DatePickerComponent} from '../_forms/date-picker/date-picker.component';
import {Router} from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
    imports: [ReactiveFormsModule, NgIf, TextInputComponent, DatePickerComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private readonly accountService = inject(AccountService);
 /*  private readonly toaster = inject(ToastrService); */
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  cancelRegister = output<boolean>();
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();
  validationErrors: string[] | undefined;
    isSubmitting = false;

  ngOnInit() {
      this.initializeForm();
      this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

    initializeForm() {
        this.registerForm = this.fb.group({
            gender: ['', Validators.required],
            username: ['', [Validators.required, usernameValidator()]],
            knownAs: ['', Validators.required],
            dateOfBirth: ['', Validators.required],
            city: ['', Validators.required],
            country: ['', Validators.required],
            password: ['', [Validators.required, passwordValidator()]],
            confirmPassword: ['', [Validators.required]]
        }, { validators: confirmPasswordValidator('password', 'confirmPassword') });
        this.registerForm.controls['password'].valueChanges.subscribe({
            next: () => {
                this.registerForm.controls['confirmPassword'].updateValueAndValidity();
            }
        })
    }

  register() {
      if (this.isSubmitting) return;
      this.isSubmitting = true;
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({dateOfBirth: dob});
    this.accountService.Register(this.registerForm.value).subscribe({
      next: _ => {
        this.router.navigateByUrl('/members')
        this.toaster.success('Registration successful');
      },
      error: (error) => {
        this.validationErrors = error.errors;
        this.toaster.error(error.error);
        this.isSubmitting = false;
      },
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
      if (!dob) return ;
      return new Date(dob).toISOString().slice(0, 10);
  }
}
