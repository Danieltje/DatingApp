import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  /* This is where we store our results in. We also initialize a new paginatedResult of type Member[] */
  

  /* We use a Service to store our data. Angular provides built in features and it's overkill in this app
     to use something like Redux, so we use the concept of an ng service.
  */

  constructor(private http: HttpClient) {}

  /* To get our paginatedResult we need to specify pagination parameters */
  getMembers(userParams: UserParams) {
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    
    // We need to pass up our parameters here. When we use http.get we get only the body of the response.
    // When we use observe it gets the full response
    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params);
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
