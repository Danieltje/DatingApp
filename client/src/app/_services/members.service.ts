import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  /* We use a Service to store our data. Angular provides built in features and it's overkill in this app
     to use something like Redux, so we use the concept of an ng service.
  */

  constructor(private http: HttpClient) {}

  getMembers() {
    /* If we have the members, then we're going to return the members from the Observable */
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;

        // map makes it so it returns an Observable
        return members;
      })
    )
  }

  getMember(username: string) {
    /* If we don't have the member make the API call, if we do; just return the member and don't make a new call */

    const member = this.members.find(x => x.username === username);
    if (member !==  undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }
}
