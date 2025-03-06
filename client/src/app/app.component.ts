import { Component, inject, OnInit, Inject } from '@angular/core';
import { ActivatedRoute , RouterOutlet } from '@angular/router';
import { NavComponent } from "./nav/nav.component";
import { AccountService } from './_services/account.service';

import { Title } from '@angular/platform-browser';
import {NgxSpinnerComponent} from 'ngx-spinner';

@Component({
  selector: 'app-root',
  standalone: true,
    imports: [NavComponent, RouterOutlet, NgxSpinnerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  constructor(private readonly titleService: Title, @Inject(ActivatedRoute) private readonly route: ActivatedRoute) {
    this.route.data.subscribe((data) => {
      this.titleService.setTitle(data['title'] || 'DatingApp');
    });
  }
  private readonly accountService = inject(AccountService);
  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) {
      return;
    }
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }
}
