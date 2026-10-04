import { Subject } from '../_models/subject';
import { Component } from '@angular/core';
import { env } from '../_env/env';
import { MATERIAL_IMPORTS } from '../_shared/material';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-create-subject',
  imports: [MATERIAL_IMPORTS, FormsModule],
  templateUrl: './create-subject.html',
  styleUrl: './create-subject.scss',
})
export class CreateSubject {
  public subject: Subject;

  constructor(
    private http: HttpClient,
    private route: Router,
    private matSnackBar: MatSnackBar,
  ) {
    this.subject = new Subject();
  }

  createSubject(): void {
    let headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: 'Bearer ' + localStorage.getItem(env.jwtToken),
    });

    this.http.post(env.subjectUri, this.subject, { headers: headers }).subscribe(
      (success) => {
        this.route.navigate(['/list-subjects']);
        console.log('::SUCCESS::', success);
        this.matSnackBar.open('Created the subject!', 'Close', { duration: 5000 });
      },
      (error) => {
        this.route.navigate(['/list-subjects']);
        console.log('::ERROR::', error);
        this.matSnackBar.open('Error happened!', 'Close', { duration: 5000 });
      },
    );
  }
}
