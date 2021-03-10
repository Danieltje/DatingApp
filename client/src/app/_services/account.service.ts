/*
An Angular service is a singleton. When we inject it into a component it will initialize and stay initialized
until the app is disposed of. The data in our store doesn't get destroyed until the app shuts down
*/

import { HttpClient } from '@angular/common/http';
import { ThisReceiver } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

// this decorator makes it possible so our services can be injected into other components/services in our app
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;

  // create an Observable to store our User in
  /* We use a special type of Observable called ReplaySubject. This is kind of like a buffer object.
     It's going to store the values inside of this object, and any time a subscriber subscribes to this
     Observable it's going to emit the last value inside of it.

     We're giving it a value of type User because that is what we still store inside of it
     Then we specify the size of our buffer; how many previous values you want inside
     It's just a User object for the current user, so we just want 1
     It's value going to be null or 1, the current user
  */
  private currentUserSource = new ReplaySubject<User>(1);

  // using a dollar$ at the end of a variable is a naming convention for Observable 
  currentUser$ = this.currentUserSource.asObservable();

  // injecting the HttpClient with a constructor
  constructor(private http: HttpClient, private presence: PresenceService) {}
  
  // the login is receiving our credentials from the login form
  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  // we receive the (user) model from our register component
  // making use of the pipe() method again because when a user registers we're considering it
  // logged in to our application
  // To repeat it; with a pipe we transform data that comes back in an Observable
  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  setCurrentUser(user: User) {
    // .next is used to set the value inside of the ReplaySubject object Observable
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
  }

  // A method to go and get the decoded token. For the admin functionality in the client.
  // We're not decoding anything really, just getting the information inside the token.
  // The token comes in 3 parts, header, payload, and the signature. We need the payload.
  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
