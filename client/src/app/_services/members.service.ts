import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, pipe } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;

  /* This is where we store our results in. We also initialize a new paginatedResult of type Member[] */
  

  /* We use a Service to store our data. Angular provides built in features and it's overkill in this app
     to use something like Redux, so we use the concept of an ng service.
  */

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  /* To get our paginatedResult we need to specify pagination parameters */
  getMembers(userParams: UserParams) {
    
    // This is actually the key; the type of query you make like female, last created. The value is the response we get from the server
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    
    // We need to pass up our parameters here. When we use http.get we get only the body of the response.
    // When we use observe it gets the full response
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)

    // Use this pipe to transform the data we get back to cache the data
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))
  }

  getMember(username: string) {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);
    
    if (member) {
      return of(member);
    }

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

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

     return this.http.get<T>(url, { observe: 'response', params }).pipe(
     map(response => {
       paginatedResult.result = response.body;
       if (response.headers.get('Pagination') !== null) {
         paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
       }
       return paginatedResult;
     })
   );
 }

 private getPaginationHeaders(pageNumber: number, pageSize: number) {
 // this serializes our params, and we can add it to our query string
   let params = new HttpParams();

 // Query string. Because the pageNumber is a string, we need to make the page toString()
   params = params.append('pageNumber', pageNumber.toString());
   params = params.append('pageSize', pageSize.toString());

   return params;
 }
}
