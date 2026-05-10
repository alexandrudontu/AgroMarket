import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

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
    private router: Router,
    private toastr: ToastrService
  ) {}

  login() {
    this.authService.login(this.model).subscribe({
      next: (res: any) => {
        this.authService.saveToken(res);
        this.toastr.success('Bine ai revenit!');
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        this.toastr.error('Eroare la autentificare.');
      }
    });
  }
}
