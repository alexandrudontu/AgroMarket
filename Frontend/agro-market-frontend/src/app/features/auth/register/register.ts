import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  imports: [FormsModule]
})
export class RegisterComponent {

  model = {
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    accountType: 'Customer'
  };

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  register() {
    this.authService.register(this.model).subscribe({
      next: (res: any) => {
        this.authService.saveToken(res);
        this.toastr.success('Înregistrare reușită!');
        this.router.navigateByUrl('/');
      },
      error: () => {
        this.toastr.error('Înregistrare eșuată.');
      }
    });
  }
}
