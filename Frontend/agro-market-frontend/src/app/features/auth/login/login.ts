import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  imports: [FormsModule]
})
export class LoginComponent {

  model = {
    email: '',
    password: ''
  };

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login() {
    this.authService.login(this.model).subscribe({
      next: (res: any) => {
        this.authService.saveToken(res);
        this.router.navigateByUrl('/');
      }
    });
  }
}
