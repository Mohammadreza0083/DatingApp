import { Component, Input, Self, Optional } from '@angular/core';
import {ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule} from '@angular/forms';
import {NgIf} from '@angular/common';

@Component({
    selector: 'app-text-input',
    standalone: true,
    templateUrl: './text-input.component.html',
    imports: [
        NgIf,
        ReactiveFormsModule
    ]
})
export class TextInputComponent implements ControlValueAccessor {
    @Input() label = '';
    @Input() type = 'text';

    constructor(@Self() public ngControl: NgControl) {
        this.ngControl.valueAccessor = this;
    }

    get control(): FormControl {
        return this.ngControl.control as FormControl;
    }

    get isInvalid(): boolean {
        return this.control?.invalid && (this.control?.touched && this.control?.dirty);
    }

    get isValid(): boolean {
        return this.control?.valid && (this.control?.touched || this.control?.dirty);
    }

    writeValue(value: any): void {
        if (this.control) {
            this.control.setValue(value, { emitEvent: false });
        }
    }

    registerOnChange(fn: any): void {
        this.control?.valueChanges.subscribe(fn);
    }

    registerOnTouched(fn: any): void {
        this.control?.markAsTouched();
    }
}
