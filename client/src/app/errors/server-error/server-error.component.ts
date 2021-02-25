import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {
  error: any;

  // inject the Router because we want access to the Router state
  // we can only access the state in the constructor here
  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();

    // the ? is an optional chaining operator
    this.error = navigation?.extras?.state?.error;
    
   }

  ngOnInit(): void {
  }

}
