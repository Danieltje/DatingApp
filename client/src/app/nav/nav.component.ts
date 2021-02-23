import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  // create a class property to store what the users enters into the form
  // model property of type any and initialise this to an empty object
  model: any = {}

  constructor() { }

  ngOnInit(): void {
  }

  login() {

    // simply log the values we receive from the form
    console.log(this.model);
  }

}
