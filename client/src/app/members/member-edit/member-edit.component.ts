import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  // This ViewChild gives us access to the template form inside the html
  // This means that when the view DOM changes, and a new child matches the selector, the property is updated
  @ViewChild('editForm') editForm: NgForm;
  member: Member;
  user: User;
  
  // we need to fetch the the current user (accountservice)
  // and with that we need to get the username, so we can get the username and go and fetch that particular member (memberservice)
  constructor(private accountService: AccountService, private memberService: MembersService,
    private toastr: ToastrService) {
    // We need to get to get the current user. Remember that it's an Observable, so we need to get it out of there
      this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe(member => {
      this.member = member;
    })
  }

  updateMember() {
    console.log(this.member);
    this.toastr.success('Profile updated successfully');
    this.editForm.reset(this.member);
  }

}
