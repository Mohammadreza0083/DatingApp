import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {member} from '../_models/member';
import {of, tap} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
    private readonly http = inject(HttpClient)
    baseUrl = environment.apiUrl;
    members = signal<member[]>([]);

    getMembers() {
        return this.http.get<member[]>(this.baseUrl + 'users').subscribe(members => {
            this.members.set(members);
        });
    }

    getMember(username: string) {
        const SelectedMember = this.members().find(m => m.username === username);
        if (SelectedMember!=undefined) return of(SelectedMember);
        return this.http.get<member>(this.baseUrl + 'users/' + username);
    }

    updateMember(member: member){
        return this.http.put(this.baseUrl + 'users', member).pipe(
            tap(()=>{
                this.members.update(members=>members.map(
                    m=>m.username===member.username?member:m
                ))
            })
        );
    }


}
