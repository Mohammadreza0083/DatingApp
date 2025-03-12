import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import {member} from '../_models/member';
import {of, tap} from 'rxjs';
import {photo} from '../_models/photo';

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

    setMainPhoto(photo: photo){
        return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id, {}).pipe(
            tap(()=>{
                this.members.update(members=>members.map(
                    m=>{
                        if (m.photos.includes(photo)){
                            m.photoUrl = photo.url;
                        }
                        return m;
                    }
                ))
            }));
    }

    deletePhoto(photo: photo){
        return this.http.delete(this.baseUrl + 'users/delete-photo/' + photo.id).pipe(
            tap(()=>{
                this.members.update(members=>members.map(
                    m=>{
                        if (m.photos.includes(photo)){
                            m.photos = m.photos.filter(p=>p.id!==photo.id);
                            m.photoUrl = photo.url;
                        }
                        return m;
                    }
                ))
            }));
    }
}
