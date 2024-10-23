import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { ThemeService } from 'src/app/services/theme.service';
import { ArticleComponent } from '../dialog/article/article.component';
import { ArticleService } from 'src/app/services/article.service';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { GlobalConstants } from 'src/app/shared/global-constants';
import { ConfirmationComponent } from '../dialog/confirmation/confirmation.component';
import { ViewArticleComponent } from '../dialog/view-article/view-article.component';

@Component({
  selector: 'app-manage-article',
  templateUrl: './manage-article.component.html',
  styleUrls: ['./manage-article.component.scss']
})
export class ManageArticleComponent implements OnInit{
  displayedColumns: string[] = ['title', 'categoryName', 'status', 'publication_date', 'edit'];
  dataSource: any;
  responseMessage:any;

  constructor(private ngxService: NgxUiLoaderService,
    private dialog:MatDialog,
    private snackbarService: SnackbarService,
    private router: Router,
    public themeService: ThemeService,
    private articleService:ArticleService
  ){}

  ngOnInit(): void {
    this.ngxService.start();
    this.tableData();
  }

  tableData(){
    this.articleService.getAllArticles().subscribe((response:any) => {
      this.ngxService.stop();
      this.dataSource = new MatTableDataSource(response);
    }, (error: any) => {
      this.ngxService.stop();
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      } else {
        this.responseMessage = GlobalConstants.genericError
      }
      this.snackbarService.openSnackBar(this.responseMessage);
    })
  }
  
  applyFilter(event:Event) {
    debugger;
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  handleAddAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data={
      action:'Add'
    };

    dialogConfig.width = "850px";
    const dialogRef = this.dialog.open(ArticleComponent, dialogConfig);
    this.router.events.subscribe(() => {
      dialogRef.close();
    });
    const res = dialogRef.componentInstance.onAddArticle.subscribe(
      (response:any) => {
        this.tableData();
      }
    )
  }

  handleViewAction(values:any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data={
      action:'View',
      data: values
    };

    dialogConfig.width = "850px";
    const dialogRef = this.dialog.open(ViewArticleComponent, dialogConfig);
    this.router.events.subscribe(() => {
      dialogRef.close();
    });
  }

  handleEditAction(values:any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data={
      action:'Edit',
      data: values
    };

    dialogConfig.width = "850px";
    const dialogRef = this.dialog.open(ArticleComponent, dialogConfig);
    this.router.events.subscribe(() => {
      dialogRef.close();
    });
    const res = dialogRef.componentInstance.onEditArticle.subscribe(
      (response:any) => {
        this.tableData();
      }
    )
  }

  onDelete(value:any){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      message: 'delete' + value.title + 'article'
    };
    const dialogRef = this.dialog.open(ConfirmationComponent, dialogConfig);
    const res = dialogRef.componentInstance.onEmitStatusChange.subscribe((response:any) => {
      this.ngxService.start();
      this.deleteProduct(value.id);
      dialogRef.close();
    })
  }

  deleteProduct(id:any){
    this.articleService.deleteArticle(id).subscribe((response:any) => {
      this.ngxService.stop();
      this.tableData();
      this.responseMessage = response.message;
      this.snackbarService.openSnackBar(this.responseMessage);
    }, (error: any) => {
      this.ngxService.stop();
      if(error.error?.message){
        this.responseMessage = error.error?.message;
      } else {
        this.responseMessage = GlobalConstants.genericError
      }
      this.snackbarService.openSnackBar(this.responseMessage);
    });
  }
}
