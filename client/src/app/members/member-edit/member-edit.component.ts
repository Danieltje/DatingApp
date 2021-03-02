import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
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
  
  // we can access Browser Events with this, so we can prevent the user from visiting another website f.e. and losing their changes
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event:any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

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
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toastr.success('Profile updated successfully');

      // update the dirty status of the form itself
      this.editForm.reset(this.member);
    })
    
  }

}
