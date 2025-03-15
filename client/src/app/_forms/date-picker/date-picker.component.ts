import {Component, Input, input, Self} from '@angular/core';
import {ControlValueAccessor, FormControl, FormsModule, NgControl, ReactiveFormsModule} from '@angular/forms';
import {BsDatepickerConfig, BsDatepickerModule} from 'ngx-bootstrap/datepicker';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-date-picker',
  standalone: true,
    imports: [BsDatepickerModule, FormsModule, NgIf, ReactiveFormsModule],
  templateUrl: './date-picker.component.html',
  styleUrl: './date-picker.component.css'
})
export class DatePickerComponent implements ControlValueAccessor{
    @Input() label = ``;
    maxDate = input<Date>();
    bsConfig?: Partial<BsDatepickerConfig>;

    constructor(@Self() public ngControl : NgControl) {
        this.ngControl.valueAccessor = this;
        this.bsConfig ={
            containerClass: 'theme-red',
            dateInputFormat: 'DD MMMM YYYY',
        }
    }

    registerOnChange(fn: any): void {
    }

    registerOnTouched(fn: any): void {
    }

    writeValue(obj: any): void {
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


}
