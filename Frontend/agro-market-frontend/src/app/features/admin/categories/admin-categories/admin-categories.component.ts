import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CategoriesService } from '../../../categories/categories.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin-categories.component.html'
})
export class AdminCategoriesComponent implements OnInit {

  categories: any[] = [];

  newCategory = '';

  constructor(private categoryService: CategoriesService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getAll()
      .subscribe({
        next: (res: any) => {
          this.categories = res;
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error(err);
        }
      });
  }

  add() {

    if (!this.newCategory.trim()) {
      return;
    }

    const body = {
      name: this.newCategory
    };

    this.categoryService.create(body)
      .subscribe({
        next: () => {

          this.newCategory = '';

          this.loadCategories();
        },
        error: (err) => {
          console.error(err);
        }
      });
  }
}