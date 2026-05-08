import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

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
    private router: Router
  ) {}

  register() {
    this.authService.register(this.model).subscribe({
      next: (res: any) => {
        this.authService.saveToken(res);
        this.router.navigateByUrl('/');
      },
      error: () => alert('Register failed')
    });
  }
}
