import { signal } from '@angular/core';
import { env } from '../_env/env';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { Register as RegisterModel } from '../_models/register';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../_shared/material';

@Component({
  selector: 'app-register',
  imports: [MATERIAL_IMPORTS, CommonModule, ReactiveFormsModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  public email: FormControl;
  public registerModel: RegisterModel;
  public acceptTermsAndConditions: boolean;

  hide = signal(true);
  clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }

  constructor(
    private http: HttpClient,
    private snackBar: MatSnackBar,
    private router: Router,
  ) {
    this.acceptTermsAndConditions = false;
    this.email = new FormControl('', [Validators.required, Validators.email]);
    this.registerModel = {
      email: '',
      password: '',
      firstName: '',
      lastName: '',
    };
  }

  get canRegister(): boolean {
    return (
      this.acceptTermsAndConditions &&
      this.registerModel.firstName.trim() !== '' &&
      this.registerModel.lastName.trim() !== '' &&
      this.registerModel.email.trim() !== '' &&
      this.registerModel.password.trim() !== '' &&
      !this.email.hasError('email')
    );
  }

  get getEmailErrorMessage(): string {
    if (this.email.hasError('required')) {
      return 'You must enter a value!';
    }

    return this.email.hasError('email') ? 'Not a valid email' : '';
  }

  sendRegisterCredentials(): void {
    this.http.put(`${env.authUri}/register`, this.registerModel).subscribe(
      (success) => {
        this.snackBar
          .open('Register was successful!', 'Close', { duration: 5000 })
          .afterDismissed()
          .subscribe(() => {
            this.router.navigate(['/login']);
          });
        console.log('::SUCCESS::', success);
      },
      (error) => {
        console.log('::ERROR::');
        this.snackBar.open(error.error.message, 'Close', { duration: 5000 });
      },
    );
  }
}
