import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  // we want to emit a value when we click on the cancel button
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  // Setting up our Reactive Form. FormGroup type tracks the value and validity state of a group of FormControl instances
  registerForm: FormGroup;
  maxDate: Date;

  constructor(private accountService: AccountService,
     private toastr: ToastrService, private fb: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();

    // Don't allow the user to pick a date less than 18 years ago (need to be 18 years and older)
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, 
        Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })
  }

  // Adding a custom validator. We compare a FormControl to another Control we "match to". In our case a password vs confirmPassword
  // If the passwords match we return null, and validation is passed, and if it doesn't match it returns a validator error isMatching true and fails
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
       ? null : {isMatching: true}
    }
  }

  register() {
    console.log(this.registerForm.value);
    //this.accountService.register(this.model).subscribe(response => {
    //  console.log(response);
    //  this.cancel();
    //}, error => {
    //  console.log(error);
    //  this.toastr.error(error.error);
   // })
  }

  cancel() {

    // we want to put the registerMode property value to false when button clicked, that's why we emit false value here
    // so we now created an event in the register comp html and ts, now we need steps in home html and ts
    this.cancelRegister.emit(false);
  }

}
