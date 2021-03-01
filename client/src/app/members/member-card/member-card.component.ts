import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  // We need to use an @Input property, we will be receiving the data from it's parent
  // which is the Member list component.
  @Input() member: Member;

  constructor() { }

  ngOnInit(): void {
  }

}
