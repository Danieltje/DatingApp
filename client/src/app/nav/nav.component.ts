import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
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

  // inject our account.service.ts into the nav component
  // if we want to access a service from here we need to make it public
  // also inject our Router into our component like service
  constructor(public accountService: AccountService, private router: Router,
     private toastr: ToastrService) { }

  ngOnInit(): void {
    
  }

  login() {
    // use the account.service to actually login our user
    this.accountService.login(this.model).subscribe(response => {
      this.router.navigateByUrl('/members');
    }, error => {
      console.log(error);
      this.toastr.error(error.error);
    } )
  }

  logout() {

    // here we actually initiate the logout function we made in the account.service.ts file
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
