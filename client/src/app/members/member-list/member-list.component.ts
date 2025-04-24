import {Component, inject, OnInit} from '@angular/core';
import {MembersService} from '../../_services/members.service';
//import {member} from '../../_models/member';
import {MemberCardComponent} from '../member-card/member-card.component';
import { pagination } from '../../_models/pagination';
import { PaginationModule} from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-member-list',
  standalone: true,
    imports: [
        MemberCardComponent,PaginationModule
    ],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent implements OnInit {
   // protected readonly
     membersService = inject(MembersService);
     pageNumber = 1;
     pageSize = 5 ;


    ngOnInit() {
        if (!this.membersService.paginatedResult()) this.loadMembers();
    }

    loadMembers() {
        this.membersService.getMembers(this.pageNumber, this.pageSize);
    }
    pageChanged(event:any) {
      if (this.pageNumber !==event.page){
       this.pageNumber = event.page;
       this.loadMembers() ;
      }
    }

}
