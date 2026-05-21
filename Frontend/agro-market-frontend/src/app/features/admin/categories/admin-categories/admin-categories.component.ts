import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CategoriesService } from '../../../categories/categories.service';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.css'
})
export class AdminCategoriesComponent implements OnInit {

  categories: any[] = [];

  newCategory = '';
  newCategoryIcon = '🛒';

  constructor(private categoryService: CategoriesService, private cdr: ChangeDetectorRef, private toastr: ToastrService) {}

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
      name: this.newCategory,
      icon: this.newCategoryIcon || '🛒'
    };

    this.categoryService.create(body)
      .subscribe({
        next: () => {

          this.newCategory = '';
          this.newCategoryIcon = '🛒';

          this.toastr.success('Categoria a fost creată cu succes!');
          this.cdr.detectChanges();
          this.loadCategories();
        },
        error: (err) => {
          console.error(err);
          this.toastr.error('Categoria nu a putut fi creată.');
        }
      });
  }
}