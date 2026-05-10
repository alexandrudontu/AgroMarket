import { Component, OnInit } from '@angular/core';
import { ProductsService } from '../../products/products.service';
import { CategoriesService } from '../../categories/categories.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import co from '@angular/common/locales/co';
import th from '@angular/common/locales/th';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-farmer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html'
})
export class FarmerDashboardComponent implements OnInit {

  products: any[] = [];
  categories: any = [];

  selectedFiles: File[] = [];
  imagePreviews: string[] = [];
  editingProductId: number | null = null;

  model: any = {
    name: '',
    description: '',
    price: null,
    quantity: null,
    imagePreviews: [],
    unitOfMeasurement: null,
    productImages: [],
    categoryId: null
  };

  constructor(
    private productService: ProductsService,
    private categoryService: CategoriesService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadProducts();

    this.categoryService.getAll()
      .subscribe(res => {
        this.categories = res;
        this.cdr.detectChanges();
      });
  }

  loadProducts() {
    this.productService.getMyProducts()
      .subscribe(res => {
        this.products = res;
        this.cdr.detectChanges();
      });
  }

  save() {
    
    // CREATE
    if (!this.editingProductId) {
    
      this.productService.create(this.model)
        .subscribe((createdProduct: any) => {
        
          // upload images
          this.uploadImages(createdProduct.id);
          this.cdr.detectChanges();
          this.toastr.success('Produs adăugat cu succes!');
        
          this.afterSave();
        });
      
      return;
    }
  
    // UPDATE
    this.productService.update(
      this.editingProductId,
      this.model
    ).subscribe(() => {
    
      // upload new images
      this.uploadImages(this.editingProductId!);
      this.cdr.detectChanges();
      this.toastr.success('Produs actualizat cu succes!');
      this.afterSave();
    });
  }

  afterSave() {

    this.resetForm();
    this.loadProducts();
  }

  edit(product: any) {
  
    this.editingProductId = product.id;

    this.model = {
      name: product.name,
      description: product.description,
      price: product.price,
      unitOfMeasurement: product.unitOfMeasurement,
      quantity: product.quantity,
      productImages: product.productImages,
      categoryId: product.categoryId
    };
  }

  delete(id: number) {

    if (!confirm('Delete product?'))
      return;

    this.productService.delete(id)
      .subscribe(() => {
        this.toastr.success('Produs șters cu succes!');
        this.loadProducts();
        this.cdr.detectChanges();
      });
  }

  resetForm() {

    this.editingProductId = null;

    this.selectedFiles = [];
    this.imagePreviews = [];

    this.model = {
      name: '',
      description: '',
      price: 0,
      quantity: 0,
      unitOfMeasurement: '',
      categoryId: null
    };
  }

  onFileSelected(event: any) {

    const files = event.target.files;

    if (!files?.length)
      return;

    this.selectedFiles = [];
    this.imagePreviews = [];

    for (let file of files) {

      this.selectedFiles.push(file);

      const reader = new FileReader();

      reader.onload = (e: any) => {
        this.imagePreviews.push(e.target.result);
      };

      reader.readAsDataURL(file);
    }
  }

  uploadImages(productId: number) {

    if (!this.selectedFiles.length) {
      this.afterSave();
      return;
    }

    let uploaded = 0;

    for (let file of this.selectedFiles) {

      this.productService
        .uploadImage(productId, file)
        .subscribe(() => {

          this.cdr.detectChanges();
          uploaded++;

          if (uploaded === this.selectedFiles.length) {
            this.afterSave();
          }
        });
    }
  }

  deleteImage(id: number) {

    this.productService.deleteImage(id)
      .subscribe(() => {
        this.cdr.detectChanges();
        this.loadProducts();
      });
  }
}