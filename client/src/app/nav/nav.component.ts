import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  // create a class property to store what the users enters into the form
  // model property of type any and initialise this to an empty object
  model: any = {}
  loggedIn: boolean;

  // inject our account.service.ts into the nav component
  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  login() {
    // use the account.service to actually login our user
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      this.loggedIn = true;
    }, error => {
      console.log(error);
    } )
  }

  logout() {
    this.loggedIn = false;
  }

}
