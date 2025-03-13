import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function usernameValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    if (!value) {
      return null;
    }

    const errors: ValidationErrors = {};
    if (!/^[A-Za-z]/.test(value)) {
      errors['startsWithLetter'] = true;
    }
    if (value.length < 3) {
      errors['minLength'] = true;
    }
    if (!/^[A-Za-z0-9]*$/.test(value)) {
      errors['noSpecialChar'] = true;
    }

    return Object.keys(errors).length ? errors : null;
  };
}
