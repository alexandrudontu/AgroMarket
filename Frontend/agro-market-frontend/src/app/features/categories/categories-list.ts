import { Component, OnInit } from '@angular/core';
import { CategoriesService } from './categories.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'categories-list',
  templateUrl: './categories-list.html',
  styleUrl: './categories-list.css',
  imports: [RouterModule, CommonModule] 
})
export class CategoriesListComponent implements OnInit {

  categories: any = [];

  constructor(private service: CategoriesService) {}

  ngOnInit() {
    this.service.getAll().subscribe(res => {
      this.categories = res;
    });
  }
}