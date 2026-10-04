import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatDivider } from '@angular/material/divider';

@Component({
  selector: 'app-footer',
  imports: [MatIcon, MatDivider],
  templateUrl: './footer.html',
  styleUrl: './footer.scss',
})
export class Footer {}
