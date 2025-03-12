import {Component, HostListener, inject, OnInit, ViewChild} from '@angular/core';
import {member} from '../../_models/member';
import {AccountService} from '../../_services/account.service';
import {MembersService} from '../../_services/members.service';
import {TabsModule} from 'ngx-bootstrap/tabs';
import {FormsModule, NgForm} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {NgClass} from '@angular/common';
import {PhotoEditorComponent} from '../photo-editor/photo-editor.component';

@Component({
  selector: 'app-member-edit',
  standalone: true,
    imports: [
        TabsModule,
        FormsModule,
        NgClass,
        PhotoEditorComponent
    ],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit{

    @ViewChild('editForm') editForm?: NgForm;
    @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any)
    {
        if(this.editForm?.dirty){
            $event.returnValue = true;
        }
    }
    member?: member;
    formDirty = false;
    private readonly accountService = inject(AccountService);
    private readonly memberService = inject(MembersService);
    private readonly toaster = inject(ToastrService);

    ngOnInit() {
        this.loadMember();
    }

    loadMember(){
        const user = this.accountService.currentUser();
        if(!user) return;
        this.memberService.getMember(user.username).subscribe({
            next: member => {
                this.member = member;
            }
        })
    }

    updateMember(){
        this.memberService.updateMember(this.editForm?.value).subscribe({
            next: _ =>{
                this.toaster.success("Successfully updated member");
                this.loadMember();
                this.editForm?.reset(this.member);
                this.editForm?.form.markAsPristine();
                this.formDirty = false;
            },
            error: error => {
                this.toaster.error('There was an error updating the member. Please try again.');
            }
        })
    }

    onMemberChange(event: member){
        this.member = event;
    }

}
