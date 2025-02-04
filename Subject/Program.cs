using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml.Linq;


internal class Program
{

    static void Main(string[] args)
    {
        //hi
        StreamWriter writer;
        writer = File.CreateText("writeTest.txt");
        writer.WriteLine("텍스트 파일 새로 쓰기 성공");
        writer.Close();

        int startSelect;

        Player p = new Player(GetPlayerName(), GetPlayerClass());
        //게임을 새로 시작한 경우
        //입력받은 플레이어 정보를 클래스에 저장.

        List<Item> items = new List<Item>();
        //아이템 리스트 생성
        List<Dungeon> dungeons = new List<Dungeon>();
        //던전 리스트 생성

        ItemSetting(ref items);
        DungeonSetting(ref dungeons);
        //아이템을 생성해 items 리스트에 집어넣어 줍니다.
        //던전도 마찬가지

        while (true)
        {
            startSelect = GetPlayerStartSelect();
            //시작메뉴에서 무엇을 할 지 결정

            if (startSelect == 1) //상태보기를 고른 경우
            {
                int statusSelect = ShowStatus(p);
                if (statusSelect == 0) //나가기를 고른 경우
                {
                    continue; //시작 메뉴로 되돌아간다.
                }
            }
            else if (startSelect == 2) //인벤토리를 고른 경우
            {
                int inventorySelect = ShowInventory(items);
                if (inventorySelect == 0) //나가기를 고른 경우
                {
                    continue; //시작메뉴로 되돌아간다.
                }
                else if (inventorySelect == 1) // 장착관리를 누른 경우
                {
                    int inventorySelectSetting = ShowInventorySetting(ref p,ref items);
                    //매개변수 수정이 가능하도록 ref를 붙여서 넣어준다.
                    if(inventorySelectSetting == 0)
                    {
                        continue;
                    }
                }
            }
            else if (startSelect == 3) //상점을 고른 경우
            {
                int storeSelect = ShowStore(ref p,ref items);
                if (storeSelect == 0) //나가기를 고른 경우
                {
                    continue; //시작메뉴로 되돌아간다.
                }
                else if (storeSelect == 1) // 아이템 구매를 누른 경우
                {
                    int storeBuySelect = ShowStoreBuy(ref p, ref items);
                    if(storeBuySelect == 0)
                    {
                        continue;
                    }
                }
                else if (storeSelect == 2) //아이템 판매를 누른 경우
                {
                    int storeSellSelect = ShowStoreSell(ref p,ref items);
                }
            }
            else if (startSelect == 4) //던전입장을 고른 경우
            {
                int dungeonSelect = ShowDungeon(ref p, dungeons);
                if(dungeonSelect == 0)
                {
                    continue;
                }
                else if (dungeonSelect == 1) //쉬운 던전을 고른 경우
                {
                    DungeonResult(ref p, dungeonSelect - 1, dungeons);
                }
                else if (dungeonSelect == 2)  //일반 던전을 고른 경우
                {
                    DungeonResult(ref p, dungeonSelect - 1, dungeons);
                }
                else if (dungeonSelect == 3) //어려운 던전을 고른 경우
                {
                    DungeonResult(ref p, dungeonSelect - 1, dungeons);
                }

            }
            else if(startSelect == 5) //휴식하기를 고른 경우
            {
                int restSelect = ShowRest(ref p);
                if(restSelect == 0)
                {
                    continue;
                }
            }
        }

    }

    static void LevelUp(ref Player p) //경험치 상승 후 레벨업 여부를 판단
    {
        if(p.exp >= p.level)
        {
            p.level = p.level + 1;
            p.attack = p.attack + 0.5f;
            p.deffense = p.deffense + 1;
            p.exp = 0;
        }

    }

    static int DungeonResult (ref Player p, int difficulty, List<Dungeon> dungeons)
    {
        int select = 0;
        Random random = new Random();

        if (p.deffense + p.item_deffense < dungeons[difficulty].NeedDeffense) //권장 방어력보다 낮다면
        {
            int fail_rand = random.Next(1, 11); //1~10의 난수 생성
            if (fail_rand <= 4) //40퍼 확률로 실패
            {
                while (true) //연산 끝나고 결과값 출력할때만 while문
                {
                    Console.WriteLine("던전에 오신 것을 환영합니다.");
                    Console.WriteLine(" ");
                    Console.WriteLine("던전입장");
                    Console.WriteLine("던전 공략에 실패하였습니다...");
                    Console.WriteLine(" ");
                    Console.WriteLine("[탐험결과]");
                    Console.WriteLine($"체력:{p.health} -> {p.health / 2}");
                    Console.WriteLine(" ");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine(" ");
                    Console.WriteLine("원하시는 행동을 입력해주세요.");


                    try
                    {
                        string temp = "";
                        temp = Console.ReadLine();
                        select = int.Parse(temp);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                        Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    }

                    if (select == 0) //나가기
                    {
                        p.health = p.health / 2;
                        Console.Clear();
                        return select;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                        Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                        Console.ReadLine();
                        Console.Clear();
                        continue;
                    }
                }
            }
        }

        //여기서 미리 던전 성공 결과 계산
        int temp_health = 0;
        int temp_gold = 0;
        
        float random_variant_health = dungeons[difficulty].NeedDeffense - (p.deffense + p.item_deffense);
        int success_rand = random.Next(20, 36); //20~35의 랜덤값 지정
        temp_health = (int)(p.health - (success_rand+random_variant_health));

        float random_variant_gold = p.attack + p.item_attack;
        float success_rand2 = random.Next((int)random_variant_gold, (int)random_variant_gold * 2);
        temp_gold = (int)p.gold + (int)((float)dungeons[difficulty].Reward * (1 + (success_rand2 * 0.01)));

        while (true)
        {

            //위의 조건을 뚫고 성공했을 경우
            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("던전 클리어");
            Console.WriteLine("축하합니다!!");
            Console.WriteLine($"{dungeons[difficulty].Name}을 클리어 하였습니다.");
            Console.WriteLine(" ");
            Console.WriteLine("[탐험결과]");
            Console.WriteLine($"체력:{p.health} -> {temp_health}");
            Console.WriteLine($"Gold:{p.gold} G -> {temp_gold} G");
            Console.WriteLine(" ");
            Console.WriteLine("0. 나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //나가기
            {
                p.exp = p.exp + 1;//플레이어의 경험치를 올려준다.

                LevelUp(ref p); //레벨업 계산

                p.health = temp_health; //임시로 계산했던 값을 돌려준다.
                p.gold = temp_gold; //임시로 계산했던 값을 돌려준다.

                Console.Clear();
                return select;
            }
            else
            {

                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }
        }

        Console.Clear();
        return select;
    }

    static int ShowDungeon(ref Player p, List<Dungeon> dungeons)
    {
        int select = 0;
        
        while(true)
        {
            Random random = new Random();

            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("던전입장");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine(" ");
            Console.WriteLine($"1. 쉬운던전     | 방어력 {dungeons[0].NeedDeffense} 이상 권장");
            Console.WriteLine($"2. 일반던전     | 방어력 {dungeons[1].NeedDeffense} 이상 권장");
            Console.WriteLine($"3. 어려운던전     | 방어력 {dungeons[2].NeedDeffense} 이상 권장");
            Console.WriteLine("0. 나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //나가기
            {
            }
            else if (select == 1) //쉬움던전
            {
            }
            else if (select == 2) //일반던전
            {
            }
            else if (select == 3) //어려움던전
            {
            }
            else {

                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            break;
        }

        Console.Clear();
        return select;
    }

    static int ShowStoreSell(ref Player p, ref List<Item> items )
    {
        int select = 0;
        float sale_rate = 0.85f;

        while(true)
        {
            int inventory_storage = 0;
            List<int> ints = new List<int>();

            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("상점 - 아이템 판매");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine(" ");
            Console.WriteLine("[보유골드]");
            Console.WriteLine($"{p.gold} G");
            Console.WriteLine(" ");
            Console.WriteLine("[아이템목록]");

            foreach (Item item in items) //아이템 목록을 뽑아내는 명령어
            {
                if (item.IsBuy) //구매한 아이템만 뜨게 해야함
                {
                    Console.Write("-");
                    Console.Write($" {inventory_storage + 1} ");

                    if (item.IsEquip == true)
                    {
                        Console.Write("[E]");
                    }


                    item.PrintInfo();
                    Console.Write($"     |  {
                        (int)((float)item.Price * sale_rate) //판매비율을 곱한 가격 (0.85퍼)
                        }G");

                    Console.WriteLine("");
                    ints.Add(items.IndexOf(item));
                    inventory_storage++;
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("0.나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //나가기를 고른 경우
            {
                //아무것도 안하고 나간다.
            }
            else if (select <= inventory_storage) //특정 장비를 고른 경우
            {

                if (items[ints[select - 1]].IsEquip == true) //아이템을 착용하고 있으면 벗고 판다.
                {
                    items[ints[select - 1]].IsEquip = false;

                    if (items[ints[select - 1]].ItemType == "무기") //벗은 아이템이 무기인 경우
                    {
                        p.item_attack = p.item_attack - items[ints[select - 1]].Effect;
                    }
                    else if (items[ints[select - 1]].ItemType == "방어구") //벗은 아이템이 방어구인 경우
                    {
                        p.item_deffense = p.item_deffense - items[ints[select - 1]].Effect;
                    }
                }

                items[ints[select - 1]].IsBuy = false;

                p.gold = p.gold + (int)((float)items[ints[select - 1]].Price * sale_rate);

                Console.WriteLine();
                Console.WriteLine($"{items[ints[select - 1]].Name}의 판매가 완료 되었습니다.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            break;

        }

        Console.Clear();
        return select;
    }
   

    static int ShowRest(ref Player p)
    {
        int select = 0;
        int restPrice = 500;

        while(true)
        {
            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("휴식하기");
            Console.WriteLine($"{restPrice} G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {p.gold} G )");
            Console.WriteLine(" ");
            Console.WriteLine("1.휴식하기");
            Console.WriteLine("0.나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //여기서 나가기
            {
                //아무것도 안 적어놓으면 알아서 나간다.
            }
            else if (select == 1) //여기서 휴식 처리
            {
                if(p.gold >= restPrice) //보유 금액이 충분하다면
                {
                    p.gold = p.gold - restPrice;
                    p.health = p.health + 100; //체력 100회복

                    if(p.health > p.maxHealth) //체력이 최대체력보다 커지면막음
                    {
                        p.health = p.maxHealth;
                    }

                    Console.WriteLine("휴식을 완료했습니다.");
                    Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                    //휴식처리, 체력 100회복
                }
                else //보유 금액이 부족하다면
                {
                    Console.WriteLine("Gold 가 부족합니다.");
                    Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                    //gold가 부족합니다 출력
                }
            }
            else //예외처리
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }


            break;
        }

        Console.Clear();
        return select;
    }

    static int ShowStoreBuy(ref Player p, ref List<Item> items)
    {
        int select = 0;
        

        while(true)
        {
            int store_storage = 0;

            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine(" ");
            Console.WriteLine("[보유골드]");
            Console.WriteLine($"{p.gold} G");
            Console.WriteLine(" ");
            Console.WriteLine("[아이템목록]");

            foreach (Item item in items) //아이템 목록을 뽑아내는 명령어
            {
                store_storage = store_storage + 1;
                Console.Write($"- {store_storage} ");

                item.PrintInfo();
                if (item.IsBuy == false)
                {
                    Console.Write($"     |  {item.Price}G");
                }
                else if (item.IsBuy == true)
                {
                    Console.Write($"     |  구매완료");
                }
                Console.WriteLine("");
            }

            Console.WriteLine(" ");
            Console.WriteLine("0.나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if(select == 0) //여기서 나가기
            {
                //아무것도 안 적어놓으면 알아서 나간다.
            }
            else if(select <= store_storage) //여기서 아이템 구매 처리
            {
                if (items[select - 1].IsBuy == true)
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                    Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                else if (items[select - 1].IsBuy == false 
                    && p.gold >= items[select - 1].Price)
                 //구매한 아이템이 아니라면 정상적으로 구매
                {
                    items[select - 1].IsBuy = true;

                    p.gold = p.gold - items[select - 1].Price;

                    Console.WriteLine("구매를 완료했습니다.");
                    Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                else if(items[select - 1].IsBuy == false
                    && p.gold < items[select - 1].Price)
                {
                    Console.WriteLine("Gold가 부족합니다.");
                    Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }
                
            }
            else //예외처리
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            break;
        }

        Console.Clear();
        return select;

    }

    static int ShowStore(ref Player p,ref List<Item> items) //상점을 보여줍니다.
    {
        int select = 0;

        while (true)
        {
            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine(" ");
            Console.WriteLine("[보유골드]");
            Console.WriteLine($"{p.gold} G");
            Console.WriteLine(" ");
            Console.WriteLine("[아이템목록]");

            foreach (Item item in items) //아이템 목록을 뽑아내는 명령어
            {
                Console.Write("-");

                item.PrintInfo();
                if (item.IsBuy == false)
                {
                    Console.Write($"     |  {item.Price}G");
                }
                else if (item.IsBuy == true)
                {
                    Console.Write($"     |  구매완료");
                }
                Console.WriteLine("");
            }

            Console.WriteLine(" ");
            Console.WriteLine("1.아이템 구매");
            Console.WriteLine("2.아이템 판매");
            Console.WriteLine("0.나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");


            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //나가기를 고른 경우
            {
                //아무것도 안하고 나간다.
            }else if (select == 1)
            {
                //아무것도 안하고 나간다.
            }
            else if(select == 2)
            {

            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            break;
        }



        Console.Clear();
        return select;
    }

    static int ShowInventorySetting(ref Player p ,ref List<Item> items) //장착관리
    {
        int select = 0;

        while (true)
        {
            int inventory_storage = 0;
            List<int> ints = new List<int>();

            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("인벤토리 - 장착관리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine(" ");
            Console.WriteLine("[아이템 목록]");

            foreach (Item item in items) //아이템 목록을 뽑아내는 명령어
            {
                if (item.IsBuy) //구매한 아이템만 뜨게 해야함
                {
                    Console.Write("-");
                    Console.Write($" {inventory_storage + 1} ");

                    if (item.IsEquip == true)
                    {
                        Console.Write("[E]");
                    }


                    item.PrintInfo();
                    Console.WriteLine("");
                    ints.Add(items.IndexOf(item));
                    inventory_storage++;
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("0.나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //나가기를 고른 경우
            {
                //아무것도 안하고 나간다.
            }
            else if (select <= inventory_storage) //특정 장비를 고른 경우
            {
                if (items[ints[select - 1]].IsEquip == true) //아이템을 착용하고 있으면 벗는다. (장착해제)
                {
                    items[ints[select - 1]].IsEquip = false;

                    if(items[ints[select - 1]].ItemType == "무기") //벗은 아이템이 무기인 경우
                    {
                        p.isWeaponEquip = false;
                        p.item_attack = p.item_attack - items[ints[select - 1]].Effect;
                    }
                    else if(items[ints[select - 1]].ItemType == "방어구") //벗은 아이템이 방어구인 경우
                    {
                        p.isArmorEquip = false;
                        p.item_deffense = p.item_deffense - items[ints[select - 1]].Effect;
                    }
                }
                else if (items[ints[select - 1]].IsEquip == false) //아이템을 벗고 있으면 착용한다. (장착)
                {

                    if (items[ints[select - 1]].ItemType == "무기") //착용한 아이템이 무기인 경우
                    {
                        //다른 무기를 착용하고 있는 경우 벗는다.
                        
                        foreach(Item item in items)
                        {
                            if(item.IsEquip == true &&  item.ItemType == "무기")
                            {
                                item.IsEquip = false;
                                p.item_attack = p.item_attack - item.Effect;
                            }
                        }
                        items[ints[select - 1]].IsEquip = true;
                        p.isWeaponEquip = true;
                        p.item_attack = p.item_attack + items[ints[select - 1]].Effect;
                    }
                    else if (items[ints[select - 1]].ItemType == "방어구") //착용한 아이템이 방어구인 경우
                    {
                        //다른 방어구를착용하고 있는 경우 벗는다.

                        foreach (Item item in items)
                        {
                            if (item.IsEquip == true && item.ItemType == "방어구")
                            {
                                item.IsEquip = false;
                                p.item_deffense = p.item_deffense - item.Effect;
                            }
                        }

                        items[ints[select - 1]].IsEquip = true;
                        p.isArmorEquip = true;
                        p.item_deffense = p.item_deffense + items[ints[select - 1]].Effect;
                    }
                }

                Console.Clear();
                continue;
            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            break;
        }

        Console.Clear();
        return select;
    }

    static int ShowInventory(List<Item> items)
    {
        int select = 0;

        while (true)
        {
            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine(" ");
            Console.WriteLine("[아이템 목록]");

            foreach (Item item in items) //아이템 목록을 뽑아내는 명령어
            {
                if (item.IsBuy) //구매한 아이템만 뜨게 해야함
                {
                    Console.Write("-");
                    if (item.IsEquip == true)
                    {
                        Console.Write("[E]");
                    }


                    item.PrintInfo();
                    Console.WriteLine("");
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("1.장착 관리");
            Console.WriteLine("0.나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //상태보기를 고른 경우
            {
            }
            else if(select == 1)
            {
            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }


            break;
        }

        Console.Clear();
        return select;
    }

    static int ShowStatus(Player p)
    {
        int select = 0;

        while (true)
        {
            p.ShowStatus();


            Console.WriteLine("0.나가기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 0) //상태보기를 고른 경우
            {
            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }


            break;
        }

        Console.Clear();
        return select;
    }

    static int GetPlayerStartSelect()
    {
        int select = 0;

        while(true)
        {
            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine(" ");
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전입장");
            Console.WriteLine("5. 휴식하기");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 1) //상태보기를 고른 경우
            {
            }
            else if (select == 2) //인벤토리를 고른 경우
            {
            }
            else if (select == 3) //상점을 고른 경우
            {
            }
            else if(select == 4) //던전입장을 고른 경우
            {
            }
            else if (select == 5) //휴식하기를 고른 경우
            {
            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }


            break;
        }

        Console.Clear();
        return select;
    }




    static int GetPlayerClass()
    {
        int playerClass = 0;
        int select;

        while (true)
        {
            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 직업을 선택해주세요.");
            Console.WriteLine(" ");
            Console.WriteLine("1. Warrior");
            Console.WriteLine("2. Magician");
            Console.WriteLine("3. Thief");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 1) //전사를 고른 경우
            {
                playerClass = (int)Enum.Parse(typeof(GameClass), "Warrior");

            }
            else if (select == 2) //마법사를 고른 경우
            {
                playerClass = (int)Enum.Parse(typeof(GameClass), "Magician");
            }
            else if (select == 3) //도적을 고른 경우
            {
                playerClass = (int)Enum.Parse(typeof(GameClass), "Thief");
            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }



            Console.Clear(); // 화면 비우기
            break;
        }

        return playerClass;
        
    }

    static string GetPlayerName() //플레이어의 이름을 입력받는 함수
    {
        string playerName;
        int select;

        while (true)
        {
            Console.WriteLine("던전에 오신 것을 환영합니다.");
            Console.WriteLine("원하시는 이름을 설정해주세요.");
            Console.WriteLine();

            try
            {
                playerName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            Console.WriteLine(" ");
            Console.WriteLine($"입력하신 이름은{playerName}입니다.");
            Console.WriteLine(" ");
            Console.WriteLine("1. 저장");
            Console.WriteLine("2. 취소");
            Console.WriteLine(" ");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(" ");

            try
            {
                string temp = "";
                temp = Console.ReadLine();
                select = int.Parse(temp);
            }
            catch (Exception e)
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            if (select == 1) //이름을 저장한 경우
            {
                //아무것도 안하고 그대로 보내준다
            }
            else if (select == 2) //이름 저장을 취소한 경우
            {
                Console.Clear(); //다시 입력하게 한다.
                continue;
            }
            else
            {
                Console.WriteLine("잘못된 값입니다. 다시 입력해주십시오.");
                Console.WriteLine("엔터를 눌러 되돌아갑니다.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }



            Console.Clear(); // 화면 비우기
            break;
        }

        return playerName;
    }

    static void DungeonSetting(ref List<Dungeon> dungeons)
    {
        Dungeon easy = new Dungeon("쉬운 던전",5, 1000);
        Dungeon normal = new Dungeon("일반 던전",11, 1700);
        Dungeon hard = new Dungeon("어려운 던전",17, 2500);

        dungeons.Add(easy);
        dungeons.Add(normal);
        dungeons.Add(hard);
    }

    static void ItemSetting(ref List<Item> items)
    {
        Item IronArmor = new Item
            (
            "무쇠갑옷", //이름
            9, //효과
            "무쇠로 만들어져 튼튼한 갑옷입니다.", //설명
            true, //구매한 아이템인가요
            "방어구", //아이템 타입은 무엇인가요
            false, //장착한 아이템인가요
            2200 //얼마인가요
            );

        Item SpartanSpear = new Item
            (
            "스파르타의 창", //이름
            7, //효과
            "스파르타의 전사들이 사용했다는 전설의 창", //설명
            true, //구매한 아이템인가요
            "무기", //아이템 타입은 무엇인가요
            false, //장착한 아이템인가요
            3200 //얼마인가요
            );

        Item OldSword = new Item
            (
            "낡은 검", //이름
            2, //효과
            "쉽게 볼 수 있는 낡은 검 입니다.", //설명
            false, //구매한 아이템인가요
            "무기", //아이템 타입은 무엇인가요
            false, //장착한 아이템인가요
            600 //얼마인가요
            );

        Item NoviceArmor = new Item
            (
            "수련자 갑옷", //이름
            5, //효과
            "수련에 도움을 주는 갑옷입니다.", //설명
            false, //구매한 아이템인가요
            "방어구", //아이템 타입은 무엇인가요
            false, //장착한 아이템인가요
            1000 //얼마인가요
            );

        Item SpartanArmor = new Item
            (
            "스파르타의 갑옷", //이름
            15, //효과
            "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", //설명
            false, //구매한 아이템인가요
            "방어구", //아이템 타입은 무엇인가요
            false, //장착한 아이템인가요
            3500 //얼마인가요
            );

        Item BronzeAxe = new Item
            (
            "청동 도끼", //이름
            5, //효과
            "어디선가 사용됐던거 같은 도끼입니다.", //설명
            false, //구매한 아이템인가요
            "무기", //아이템 타입은 무엇인가요
            false, //장착한 아이템인가요
            1500 //얼마인가요
            );

        Item MyItem = new Item
            (
            "나만의 아이템", //이름
            100, //효과
            "나만의 아이템입니다. 겁나 셉니다.", //설명
            false, //구매한 아이템인가요
            "무기", //아이템 타입은 무엇인가요
            false, //장착한 아이템인가요
            9999 //얼마인가요
            );


        items.Add(IronArmor); //아이템 클래스 생성후 리스트에 삽입
        items.Add(SpartanSpear); //아이템 클래스 생성후 리스트에 삽입
        items.Add(OldSword); //아이템 클래스 생성후 리스트에 삽입
        items.Add(NoviceArmor);//아이템 클래스 생성후 리스트에 삽입
        items.Add(SpartanArmor);
        items.Add(BronzeAxe);
        items.Add(MyItem);
    }

}
public class Player //플레이어에 관한 정보를 저장하는 클래스
{
    public string Name { get; set; } //플레이어의 이름을 입력받으면 이곳에 저장
    public int PlayerClass { get; set; } //플레이어의 직업이 무엇인지 이곳에 저장

    //플레이어의 정보들
    public int level { get; set; } = 1;
    public int exp { get; set; } = 0;
    public float attack { get; set; }  = 10; //총 공격력은 attack + item_attack
    public float item_attack { get; set; } = 0;
    public float deffense { get; set; } = 5; //총 방어력은 deffense + item_deffense
    public float item_deffense { get; set; } = 0;
    public int maxHealth { get; set; } = 100;
    public int health { get; set; } = 100;
    public float gold { get; set; }  = 300000.0f;
    public bool isWeaponEquip { get; set; } = false;
    public bool isArmorEquip { get; set; } = false;

    public Player(string name, int playerClass)
    {
        Name = name;
        PlayerClass = playerClass;
    }

    public void ShowStatus()
    {
        Console.WriteLine("던전에 오신 것을 환영합니다.");
        Console.WriteLine(" ");
        Console.WriteLine("상태보기");
        Console.WriteLine("캐릭터의 정보가 표시됩니다.");
        Console.WriteLine(" ");
        Console.WriteLine($"Lv.{level.ToString("D2")}");
        Console.WriteLine($"Chad ( {Enum.GetName(typeof(GameClass), PlayerClass)} )");
        Console.Write($"공격력:{attack + item_attack}");
        if(item_attack>0)
        {
            Console.WriteLine($"(+{item_attack})");
        }else
        {
            Console.WriteLine("");
        }
        Console.Write($"방어력:{deffense + item_deffense}");
        if (item_deffense > 0)
        {
            Console.WriteLine($"(+{item_deffense})");
        }
        else
        {
            Console.WriteLine("");
        }
        Console.WriteLine($"현재 체력:{health}");
        Console.WriteLine($"최대 체력:{maxHealth}");
        Console.WriteLine($"Gold :{gold} G");
        Console.WriteLine(" ");

    }


    public void debug()
    {

    }

}

public class Dungeon
{
    public string Name { get; set; }
    public int NeedDeffense { get; set; }
    public int Reward { get; set; }

    public Dungeon(string name ,int needDeffense, int reward)
    {
        Name = name;
        NeedDeffense = needDeffense;
        Reward = reward;
    }

}

public class Item //아이템 관리용 클래스
{
    public string Name { get; set; }
    public int Effect { get; set; }
    public string Info { get; set; }
    public bool IsBuy { get; set; }
    public bool IsSell { get; set; }
    public string ItemType { get; set; }
    public bool IsEquip { get; set; }
    public int Price { get; set; }

    public Item(string name, int effect, string info, bool isBuy, string itemType, bool isEquip, int price)
    {
        Name = name;
        Effect = effect;
        Info = info;
        IsBuy = isBuy;
        ItemType = itemType;
        IsEquip = isEquip;
        Price = price;
    }

    public void PrintInfo()
    {
        Console.Write($" {Name}");
        Console.Write("   |");
        if (ItemType == "방어구")
        {
            Console.Write($" 방어력 +{Effect}");
        }
        else if (ItemType == "무기")
        {
            Console.Write($" 공격력 +{Effect}");
        }
        Console.Write("   |");
        Console.Write($" {Info}");
    }

    public void debug()
    {
        Console.WriteLine("인벤토리 작동중");
    }
}

public enum GameClass
{
    Warrior, // 0
    Magician, // 1
    Thief // 2
}

