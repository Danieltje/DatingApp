import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
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

  constructor(private accountService: AccountService,
     private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      // A FormGroup contains Form controls
      username: new FormControl(),
      password: new FormControl(),
      confirmPassword: new FormControl()
    })
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
