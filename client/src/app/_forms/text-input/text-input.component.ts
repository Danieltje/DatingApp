import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})

/* We replaced OnInit interface for the ControlValueAccessor interface
   This acts as a bridge between the Forms API and the native DOM
*/
export class TextInputComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() type = 'text';

  /* We want to implement a ControlValueAccessor
     In the register.component.ts we have (Form)Controls
     We want to access those controls inside this component
  */

  /* We need to inject the Control into the constructor of this component
     We use a special decorator called @Self. This marks that this component will be used locally
     Angular will not 'cache' it to sum it up

     NgControl is the base class that all the FormControl directives extend
     By adding the .valueAccessor we got access to our Control inside our component when we use it
   */
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
   }

  /* We empty the code in the functions because we pass through to these methods in the template
     It doesn't matter if we enter something or we don't
     The functions will be created by the ControlValueAccessor itself
  */

  writeValue(obj: any): void {  
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void { 
  }
  
}
