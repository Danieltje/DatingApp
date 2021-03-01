import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  /* We're storing our token as part of our current user inside our AccountService
     That's why we bring our AccountService and inject it in the constructor here
  */
  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    // our currentUser is an Observable. There's a token inside there and we want to get that
    // we need to get the currentuser outside of that observable
    let currentUser: User;

    // here we say: we want to "take", and we want to take (1) thing from this Observable, and then we subscribe
    // what we say here is that we want to complete after we've received 1 of these current users
    // we don't need to unsubscribe because once an Observable has completed we are technically not subscribed to it anymore
    // the user will contain null (no user) or it will contain a value (a current user)
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => currentUser = user);

    // This will attach our token for every request when we're logged in, and send that with the request
    if (currentUser) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      })
    }

    return next.handle(request);
  }
}
