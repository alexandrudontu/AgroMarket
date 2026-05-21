import { Component, OnInit } from '@angular/core';
import { CategoriesService } from './categories.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import sw from '@angular/common/locales/sw';

@Component({
  selector: 'categories-list',
  templateUrl: './categories-list.html',
  styleUrls: ['./categories-list.css'],
  imports: [RouterModule, CommonModule] 
})
export class CategoriesListComponent implements OnInit {

  categories: any = [];

  constructor(private service: CategoriesService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.service.getAll().subscribe(res => {
      this.categories = res;
      this.cdr.detectChanges();
    });
  }
}