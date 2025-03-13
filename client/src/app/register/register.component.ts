import {Component, output, inject, OnInit} from '@angular/core';
import {
    FormBuilder,
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import {JsonPipe, NgIf} from '@angular/common';
import {passwordValidator} from '../_validators/password.validator';
import {usernameValidator} from '../_validators/username.validator';
import {confirmPasswordValidator} from '../_validators/confrim-password.validator';
import {TextInputComponent} from '../_forms/text-input/text-input.component';

@Component({
  selector: 'app-register',
  standalone: true,
    imports: [ReactiveFormsModule, JsonPipe, NgIf, TextInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  private readonly accountService = inject(AccountService);
  private readonly toaster = inject(ToastrService);
  private readonly fb = inject(FormBuilder);
  cancelRegister = output<boolean>();
  registerModel: any = {};
  registerForm: FormGroup = new FormGroup({});

  ngOnInit() {
      this.initializeForm();
  }

    initializeForm() {
        this.registerForm = this.fb.group({
            username: ['', [Validators.required, usernameValidator()]],
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
      console.log(this.registerForm.value);
    // this.accountService.Register(this.registerModel).subscribe({
    //   next: (response) => {
    //     console.log(response);
    //     this.toaster.success('Registration successful');
    //     this.cancel();
    //   },
    //   error: (error) => {
    //     this.toaster.error(error.error);
    //   },
    // });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

    protected readonly confirmPasswordValidator = confirmPasswordValidator;
}
