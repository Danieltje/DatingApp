import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;

  constructor() { }

  ngOnInit(): void {
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  // this will take an event of type boolean as parameter because we emit/output this from the register component
  // then we say, ok put the registerMode property to false again which is in this home comp ts 
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

}
