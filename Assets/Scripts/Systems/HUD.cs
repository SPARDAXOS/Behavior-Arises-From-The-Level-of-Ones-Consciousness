using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Transform PCH1Elements = null;
    Transform PCH2Elements = null;
    Transform PCH3Elements = null;
                          
    Transform ECH1Elements = null;
    Transform ECH2Elements = null;
    Transform ECH3Elements = null;

    public Transform PCH1StatusAilments = null;
    public Transform PCH2StatusAilments = null;
    public Transform PCH3StatusAilments = null;
    
    public Transform ECH1StatusAilments = null;
    public Transform ECH2StatusAilments = null;
    public Transform ECH3StatusAilments = null;


    private Image[] PCHHealthBars = new Image[3];
    private Image[] ECHHealthBars = new Image[3];

    private Text[] PCHHealthBarsTexts = new Text[3];
    private Text[] ECHHealthBarsTexts = new Text[3];

    private Image[] PCHManaBars = new Image[3];
    private Image[] ECHManaBars = new Image[3];

    private Text[] PCHManaBarsTexts = new Text[3];
    private Text[] ECHManaBarsTexts = new Text[3];

    private Text[] PCHActionTexts = new Text[3];
    private Text[] ECHActionTexts = new Text[3];

    private Image[] PCHPortraits = new Image[3];
    private Image[] ECHPortraits = new Image[3];

    private Text[] PCHAggroCountersTexts = new Text[3];
    private Text[] ECHAggroCountersTexts = new Text[3];

    public Text[] PCH1BuffTexts = new Text[2];
    public Text[] PCH2BuffTexts = new Text[2];
    public Text[] PCH3BuffTexts = new Text[2];

    private Text[] PCH1DebuffTexts = new Text[2];
    private Text[] PCH2DebuffTexts = new Text[2];
    private Text[] PCH3DebuffTexts = new Text[2];

    private Text[] ECH1BuffTexts = new Text[2];
    private Text[] ECH2BuffTexts = new Text[2];
    private Text[] ECH3BuffTexts = new Text[2];

    private Text[] ECH1DebuffTexts = new Text[2];
    private Text[] ECH2DebuffTexts = new Text[2];
    private Text[] ECH3DebuffTexts = new Text[2];

    private Image SuperBarImage = null;

    private void SetupElements()
    {
        PCH1Elements = transform.Find("PCH1");
        PCH2Elements = transform.Find("PCH2");
        PCH3Elements = transform.Find("PCH3");

        ECH1Elements = transform.Find("ECH1");
        ECH2Elements = transform.Find("ECH2");
        ECH3Elements = transform.Find("ECH3");
    }
    private void SetupStatusAilments()
    {
        PCH1StatusAilments = PCH1Elements.Find("StatusAilmentsParent");
        PCH2StatusAilments = PCH2Elements.Find("StatusAilmentsParent");
        PCH3StatusAilments = PCH3Elements.Find("StatusAilmentsParent");
                       
        ECH1StatusAilments = ECH1Elements.Find("StatusAilmentsParent");
        ECH2StatusAilments = ECH2Elements.Find("StatusAilmentsParent");
        ECH3StatusAilments = ECH3Elements.Find("StatusAilmentsParent");
    }
    private void SetupPCHStatusAilmentsTexts()
    {
        PCH1BuffTexts[0] = PCH1StatusAilments.Find("Buff1").GetComponent<Text>();
        PCH1BuffTexts[1] = PCH1StatusAilments.Find("Buff2").GetComponent<Text>();

        PCH2BuffTexts[0] = PCH2StatusAilments.Find("Buff1").GetComponent<Text>();
        PCH2BuffTexts[1] = PCH2StatusAilments.Find("Buff2").GetComponent<Text>();

        PCH3BuffTexts[0] = PCH3StatusAilments.Find("Buff1").GetComponent<Text>();
        PCH3BuffTexts[1] = PCH3StatusAilments.Find("Buff2").GetComponent<Text>();


        PCH1BuffTexts[0].text = "";
        PCH1BuffTexts[1].text = "";

        PCH2BuffTexts[0].text = "";
        PCH2BuffTexts[1].text = "";

        PCH3BuffTexts[0].text = "";
        PCH3BuffTexts[1].text = "";


        PCH1DebuffTexts[0] = PCH1StatusAilments.Find("Debuff1").GetComponent<Text>();
        PCH1DebuffTexts[1] = PCH1StatusAilments.Find("Debuff2").GetComponent<Text>();

        PCH2DebuffTexts[0] = PCH2StatusAilments.Find("Debuff1").GetComponent<Text>();
        PCH2DebuffTexts[1] = PCH2StatusAilments.Find("Debuff2").GetComponent<Text>();

        PCH3DebuffTexts[0] = PCH3StatusAilments.Find("Debuff1").GetComponent<Text>();
        PCH3DebuffTexts[1] = PCH3StatusAilments.Find("Debuff2").GetComponent<Text>();


        PCH1DebuffTexts[0].text = "";
        PCH1DebuffTexts[1].text = "";
                    
        PCH2DebuffTexts[0].text = "";
        PCH2DebuffTexts[1].text = "";
                        
        PCH3DebuffTexts[0].text = "";
        PCH3DebuffTexts[1].text = "";
    }
    private void SetupECHStatusAilmentsTexts()
    {
        ECH1BuffTexts[0] = ECH1StatusAilments.Find("Buff1").GetComponent<Text>();
        ECH1BuffTexts[1] = ECH1StatusAilments.Find("Buff2").GetComponent<Text>();
                           
        ECH2BuffTexts[0] = ECH2StatusAilments.Find("Buff1").GetComponent<Text>();
        ECH2BuffTexts[1] = ECH2StatusAilments.Find("Buff2").GetComponent<Text>();
                           
        ECH3BuffTexts[0] = ECH3StatusAilments.Find("Buff1").GetComponent<Text>();
        ECH3BuffTexts[1] = ECH3StatusAilments.Find("Buff2").GetComponent<Text>();


        ECH1BuffTexts[0].text = "";
        ECH1BuffTexts[1].text = "";
                      
        ECH2BuffTexts[0].text = "";
        ECH2BuffTexts[1].text = "";
                         
        ECH3BuffTexts[0].text = "";
        ECH3BuffTexts[1].text = "";


        ECH1DebuffTexts[0] = ECH1StatusAilments.Find("Debuff1").GetComponent<Text>();
        ECH1DebuffTexts[1] = ECH1StatusAilments.Find("Debuff2").GetComponent<Text>();
                           
        ECH2DebuffTexts[0] = ECH2StatusAilments.Find("Debuff1").GetComponent<Text>();
        ECH2DebuffTexts[1] = ECH2StatusAilments.Find("Debuff2").GetComponent<Text>();
                            
        ECH3DebuffTexts[0] = ECH3StatusAilments.Find("Debuff1").GetComponent<Text>();
        ECH3DebuffTexts[1] = ECH3StatusAilments.Find("Debuff2").GetComponent<Text>();


        ECH1DebuffTexts[0].text = "";
        ECH1DebuffTexts[1].text = "";
                             
        ECH2DebuffTexts[0].text = "";
        ECH2DebuffTexts[1].text = "";
                              
        ECH3DebuffTexts[0].text = "";
        ECH3DebuffTexts[1].text = "";
    }

    private void SetupHealthBars()
    {
        Transform HealthBarParent = null;
        Transform HealthBar = null;
        Image ImageComponent = null;
        Transform CountText = null;
        Text CountTextComponent = null;

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        HealthBarParent = PCH1Elements.Find("HealthBarParent");
                    }
                    break;
                case 1:
                    {
                        HealthBarParent = PCH2Elements.Find("HealthBarParent");
                    }
                    break;
                case 2:
                    {
                        HealthBarParent = PCH3Elements.Find("HealthBarParent");
                    }
                    break;

            }

            HealthBar = HealthBarParent.Find("HealthBar");
            ImageComponent = HealthBar.GetComponent<Image>();
            PCHHealthBars[i] = ImageComponent;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        HealthBarParent = ECH1Elements.Find("HealthBarParent");
                    }
                    break;
                case 1:
                    {
                        HealthBarParent = ECH2Elements.Find("HealthBarParent");
                    }
                    break;
                case 2:
                    {
                        HealthBarParent = ECH3Elements.Find("HealthBarParent");
                    }
                    break;

            }

            HealthBar = HealthBarParent.Find("HealthBar");
            ImageComponent = HealthBar.GetComponent<Image>();
            ECHHealthBars[i] = ImageComponent;
        }

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        HealthBarParent = PCH1Elements.Find("HealthBarParent");
                    }
                    break;
                case 1:
                    {
                        HealthBarParent = PCH2Elements.Find("HealthBarParent");
                    }
                    break;
                case 2:
                    {
                        HealthBarParent = PCH3Elements.Find("HealthBarParent");
                    }
                    break;

            }
            CountText = HealthBarParent.Find("CountText");
            CountTextComponent = CountText.GetComponent<Text>();
            PCHHealthBarsTexts[i] = CountTextComponent;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        HealthBarParent = ECH1Elements.Find("HealthBarParent");
                    }
                    break;
                case 1:
                    {
                        HealthBarParent = ECH2Elements.Find("HealthBarParent");
                    }
                    break;
                case 2:
                    {
                        HealthBarParent = ECH3Elements.Find("HealthBarParent");
                    }
                    break;

            }
            CountText = HealthBarParent.Find("CountText");
            CountTextComponent = CountText.GetComponent<Text>();
            ECHHealthBarsTexts[i] = CountTextComponent;
        }
    }
    private void SetupManaBars()
    {
        Transform ManaBarParent = null;
        Transform ManaBar = null;
        Image ImageComponent = null;
        Transform CountText = null;
        Text CountTextComponent = null;

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        ManaBarParent = PCH1Elements.Find("ManaBarParent");
                    }
                    break;
                case 1:
                    {
                        ManaBarParent = PCH2Elements.Find("ManaBarParent");
                    }
                    break;
                case 2:
                    {
                        ManaBarParent = PCH3Elements.Find("ManaBarParent");
                    }
                    break;

            }

            ManaBar = ManaBarParent.Find("ManaBar");
            ImageComponent = ManaBar.GetComponent<Image>();
            PCHManaBars[i] = ImageComponent;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        ManaBarParent = ECH1Elements.Find("ManaBarParent");
                    }
                    break;
                case 1:
                    {
                        ManaBarParent = ECH2Elements.Find("ManaBarParent");
                    }
                    break;
                case 2:
                    {
                        ManaBarParent = ECH3Elements.Find("ManaBarParent");
                    }
                    break;

            }

            ManaBar = ManaBarParent.Find("ManaBar");
            ImageComponent = ManaBar.GetComponent<Image>();
            ECHManaBars[i] = ImageComponent;
        }

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        ManaBarParent = PCH1Elements.Find("ManaBarParent");
                    }
                    break;
                case 1:
                    {
                        ManaBarParent = PCH2Elements.Find("ManaBarParent");
                    }
                    break;
                case 2:
                    {
                        ManaBarParent = PCH3Elements.Find("ManaBarParent");
                    }
                    break;

            }
            CountText = ManaBarParent.Find("CountText");
            CountTextComponent = CountText.GetComponent<Text>();
            PCHManaBarsTexts[i] = CountTextComponent;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        ManaBarParent = ECH1Elements.Find("ManaBarParent");
                    }
                    break;
                case 1:
                    {
                        ManaBarParent = ECH2Elements.Find("ManaBarParent");
                    }
                    break;
                case 2:
                    {
                        ManaBarParent = ECH3Elements.Find("ManaBarParent");
                    }
                    break;

            }
            CountText = ManaBarParent.Find("CountText");
            CountTextComponent = CountText.GetComponent<Text>();
            ECHManaBarsTexts[i] = CountTextComponent;
        }
    }
    private void SetupActionTexts()
    {
        Transform CurrentActionParent = null;
        Transform CurrentActionText = null;
        Text TextComponent = null;

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        CurrentActionParent = PCH1Elements.Find("CurrentActionParent");
                    }
                    break;
                case 1:
                    {
                        CurrentActionParent = PCH2Elements.Find("CurrentActionParent");
                    }
                    break;
                case 2:
                    {
                        CurrentActionParent = PCH3Elements.Find("CurrentActionParent");
                    }
                    break;

            }

            CurrentActionText = CurrentActionParent.Find("CurrentActionText");
            TextComponent = CurrentActionText.GetComponent<Text>();
            PCHActionTexts[i] = TextComponent;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        CurrentActionParent = ECH1Elements.Find("CurrentActionParent");
                    }
                    break;
                case 1:
                    {
                        CurrentActionParent = ECH2Elements.Find("CurrentActionParent");
                    }
                    break;
                case 2:
                    {
                        CurrentActionParent = ECH3Elements.Find("CurrentActionParent");
                    }
                    break;

            }

            CurrentActionText = CurrentActionParent.Find("CurrentActionText");
            TextComponent = CurrentActionText.GetComponent<Text>();
            ECHActionTexts[i] = TextComponent;
        }
    }
    private void SetupPortraits()
    {
        Transform Portrait = null;
        Image ImageComponent = null;

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        Portrait = PCH1Elements.Find("Portrait");
                    }
                    break;
                case 1:
                    {
                        Portrait = PCH2Elements.Find("Portrait");
                    }
                    break;
                case 2:
                    {
                        Portrait = PCH3Elements.Find("Portrait");
                    }
                    break;

            }

            ImageComponent = Portrait.GetComponent<Image>();
            PCHPortraits[i] = ImageComponent;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        Portrait = ECH1Elements.Find("Portrait");
                    }
                    break;
                case 1:
                    {
                        Portrait = ECH2Elements.Find("Portrait");
                    }
                    break;
                case 2:
                    {
                        Portrait = ECH3Elements.Find("Portrait");
                    }
                    break;

            }

            ImageComponent = Portrait.GetComponent<Image>();
            ECHPortraits[i] = ImageComponent;
        }
    }
    private void SetupAggroCountersTexts()
    {
        Transform AggroCounter = null;
        Text TextComponent = null;

        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        AggroCounter = PCH1Elements.Find("AggroCounter");
                    }
                    break;
                case 1:
                    {
                        AggroCounter = PCH2Elements.Find("AggroCounter");
                    }
                    break;
                case 2:
                    {
                        AggroCounter = PCH3Elements.Find("AggroCounter");
                    }
                    break;

            }

            TextComponent = AggroCounter.GetComponent<Text>();
            PCHAggroCountersTexts[i] = TextComponent;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        AggroCounter = ECH1Elements.Find("AggroCounter");
                    }
                    break;
                case 1:
                    {
                        AggroCounter = ECH2Elements.Find("AggroCounter");
                    }
                    break;
                case 2:
                    {
                        AggroCounter = ECH3Elements.Find("AggroCounter");
                    }
                    break;

            }

            TextComponent = AggroCounter.GetComponent<Text>();
            ECHAggroCountersTexts[i] = TextComponent;
        }
    }
    private void SetupSuperBar()
    {
        Transform PlayerSuperBar = null; 
        Transform SuperBar = null;
        Image ImageComponent = null;

        PlayerSuperBar = transform.Find("PlayerSuperBar");
        if (!PlayerSuperBar)
        {
            Debug.LogError("PlayerSuperBar reference is null - HUD - SetupSuperBar");
            return;
        }

        SuperBar = PlayerSuperBar.Find("SuperBar");
        if (!SuperBar)
        {
            Debug.LogError("SuperBar reference is null - HUD - SetupSuperBar");
            return;
        }

        ImageComponent = SuperBar.GetComponent<Image>();
        if (!ImageComponent)
        {
            Debug.LogError("ImageComponent reference is null - HUD - SetupSuperBar");
            return;
        }

        SuperBarImage = ImageComponent;
    }

    //For Debugging BAD NAME MAYBE GET RID OF THEM OR CHANGE NAME idk
    private void ValidateHealthBars()
    {
        if (!PCHHealthBars[0])
            Debug.LogError("PCHHealthBar0 is null");
        if (!PCHHealthBars[1])
            Debug.LogError("PCHHealthBar1 is null");
        if (!PCHHealthBars[2])
            Debug.LogError("PCHHealthBar2 is null");

        if (!ECHHealthBars[0])
            Debug.LogError("ECHHealthBar0 is null");
        if (!ECHHealthBars[1])
            Debug.LogError("ECHHealthBar1 is null");
        if (!ECHHealthBars[2])
            Debug.LogError("ECHHealthBar2 is null");

        if (!PCHHealthBarsTexts[0])
            Debug.LogError("PCHHealthBarsText0 is null");
        if (!PCHHealthBarsTexts[1])
            Debug.LogError("PCHHealthBarsText1 is null");
        if (!PCHHealthBarsTexts[2])
            Debug.LogError("PCHHealthBarsText2 is null");

        if (!ECHHealthBarsTexts[0])
            Debug.LogError("ECHHealthBarsText0 is null");
        if (!ECHHealthBarsTexts[1])
            Debug.LogError("ECHHealthBarsText1 is null");
        if (!ECHHealthBarsTexts[2])
            Debug.LogError("ECHHealthBarsText2 is null");
    }
    private void ValidatManaBars()
    {
        if (!PCHManaBars[0])
            Debug.LogError("PCHManaBar0 is null");
        if (!PCHManaBars[1])
            Debug.LogError("PCHManaBar1 is null");
        if (!PCHManaBars[2])
            Debug.LogError("PCHManaBar2 is null");

        if (!ECHManaBars[0])
            Debug.LogError("ECHManaBar0 is null");
        if (!ECHManaBars[1])
            Debug.LogError("ECHManaBar1 is null");
        if (!ECHManaBars[2])
            Debug.LogError("ECHManaBar2 is null");

        if (!PCHManaBarsTexts[0])
            Debug.LogError("PCHManaBarsText0 is null");
        if (!PCHManaBarsTexts[1])
            Debug.LogError("PCHManaBarsText1 is null");
        if (!PCHManaBarsTexts[2])
            Debug.LogError("PCHManaBarsText2 is null");

        if (!ECHManaBarsTexts[0])
            Debug.LogError("ECHManaBarsText0 is null");
        if (!ECHManaBarsTexts[1])
            Debug.LogError("ECHManaBarsText1 is null");
        if (!ECHManaBarsTexts[2])
            Debug.LogError("ECHManaBarsText2 is null");
    }
    private void ValidateActionTexts()
    {
        if (!PCHActionTexts[0])
            Debug.LogError("PCHActionTexts0 is null");
        if (!PCHActionTexts[1])
            Debug.LogError("PCHActionTexts1 is null");
        if (!PCHActionTexts[2])
            Debug.LogError("PCHActionTexts2 is null");

        if (!ECHActionTexts[0])
            Debug.LogError("ECHActionTexts0 is null");
        if (!ECHActionTexts[1])
            Debug.LogError("ECHActionTexts1 is null");
        if (!ECHActionTexts[2])
            Debug.LogError("ECHActionTexts2 is null");
    }
    private void ValidatePortraits()
    {
        if (!PCHPortraits[0])
            Debug.LogError("PCHPortraits0 is null");
        if (!PCHPortraits[1])
            Debug.LogError("PCHPortraits1 is null");
        if (!PCHPortraits[2])
            Debug.LogError("PCHPortraits2 is null");

        if (!ECHPortraits[0])
            Debug.LogError("ECHPortraits0 is null");
        if (!ECHPortraits[1])
            Debug.LogError("ECHPortraits1 is null");
        if (!ECHPortraits[2])
            Debug.LogError("ECHPortraits2 is null");
    }


    private bool ValidateIndex(int collectionLength, int index)
    {
        if (index < 0 || index >= collectionLength)
            return false;
        return true;
    }


    public void UpdateSuperBar(float amount)
    {
        SuperBarImage.fillAmount = amount;
    }


    public void UpdatePCHHealthBar(int index, float cap, float current)
    {
        if (!ValidateIndex(PCHHealthBars.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to UpdatePCHHealthBar - PCHHealthBars - " + index);
            return;
        }
        PCHHealthBars[index].fillAmount = current/cap;
        if (!ValidateIndex(PCHHealthBarsTexts.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to UpdatePCHHealthBar - PCHHealthBarsTexts - " + index);
            return;
        }
        PCHHealthBarsTexts[index].text = cap.ToString() + "/" + current.ToString();
    }
    public void UpdateECHHealthBar(int index, float cap, float current)
    {
        if (!ValidateIndex(ECHHealthBars.Length, index))
        {
            Debug.LogError("Invalid ECH index sent to UpdateECHHealthBar - ECHHealthBars - " + index);
            return;
        }
        ECHHealthBars[index].fillAmount = current / cap;
        if (!ValidateIndex(ECHHealthBarsTexts.Length, index))
        {
            Debug.LogError("Invalid ECH index sent to UpdateECHHealthBar - ECHHealthBarsTexts - " + index);
            return;
        }
        ECHHealthBarsTexts[index].text = cap.ToString() + "/" + current.ToString();
    }

    public void UpdatePCHManaBar(int index, float cap, float current)
    {
        if (!ValidateIndex(PCHManaBars.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to UpdatePCHManaBar - PCHManaBars - " + index);
            return;
        }
        PCHManaBars[index].fillAmount = current / cap;
        if (!ValidateIndex(PCHManaBarsTexts.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to UpdatePCHManaBar - PCHManaBarsTexts - " + index);
            return;
        }
        PCHManaBarsTexts[index].text = cap.ToString() + "/" + current.ToString();
    }
    public void UpdateECHManaBar(int index, float cap, float current)
    {
        if (!ValidateIndex(ECHManaBars.Length, index))
        {
            Debug.LogError("Invalid ECH index sent to UpdateECHManaBar - ECHManaBars - " + index);
            return;
        }
        ECHManaBars[index].fillAmount = current / cap;
        if (!ValidateIndex(ECHManaBarsTexts.Length, index))
        {
            Debug.LogError("Invalid ECH index sent to UpdateECHManaBar - ECHManaBarsTexts - " + index);
            return;
        }
        ECHManaBarsTexts[index].text = cap.ToString() + "/" + current.ToString();
    }

    public void UpdatePCHActionText(int index, string action)
    {
        if (!ValidateIndex(PCHActionTexts.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to UpdatePCHActionText - PCHActionTexts - " + index);
            return;
        }

        PCHActionTexts[index].text = action;
    }
    public void UpdateECHActionText(int index, string action)
    {
        if (!ValidateIndex(ECHActionTexts.Length, index))
        {
            Debug.LogError("Invalid ECH index sent to UpdateECHActionText - ECHActionTexts - " + index);
            return;
        }
        ECHActionTexts[index].text = action;
    }

    public void UpdatePCHAggroCounter(int index, float amount) 
    {
        if (!ValidateIndex(PCHAggroCountersTexts.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to UpdatePCHAggroCounter - PCHAggroCountersTexts - " + index);
            return;
        }
        PCHAggroCountersTexts[index].text = "Aggro: " + amount.ToString();
    }
    public void UpdateECHAggroCounter(int index, float amount)
    {
        if (!ValidateIndex(ECHAggroCountersTexts.Length, index))
        {
            Debug.LogError("Invalid ECH index sent to UpdateECHAggroCounter - ECHAggroCountersTexts - " + index);
            return;
        }
        ECHAggroCountersTexts[index].text = "Aggro: " + amount.ToString();
    }

    public void SetPCHPortrait(int index, Sprite portrait)
    {
        if(!ValidateIndex(PCHPortraits.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to SetPCHPortrait - " + index);
            return;
        }
        if (!portrait)
        {
            Debug.LogError("Invalid portrait sent to SetPCHPortrait");
            return;
        }

        PCHPortraits[index].sprite = portrait;
    }
    public void SetECHPortrait(int index, Sprite portrait)
    {
        if (!ValidateIndex(ECHPortraits.Length, index))
        {
            Debug.LogError("Invalid PCH index sent to SetECHPortrait - " + index);
            return;
        }
        if (!portrait)
        {
            Debug.LogError("Invalid portrait sent to SetECHPortrait");
            return;
        }

        ECHPortraits[index].sprite = portrait;
    }

    public void SetPCHElementsState(int index, bool state)
    {
        switch (index)
        {
            case 0:
                {
                    PCH1Elements.gameObject.SetActive(state);
                }
                break;
            case 1:
                {
                    PCH2Elements.gameObject.SetActive(state);
                }
                break;
            case 2:
                {
                    PCH3Elements.gameObject.SetActive(state);
                }
                break;
            default:
                {
                    Debug.LogError("Invalid index sent to SetPCHElementsState - " + index);
                }break;

        }
    }
    public void SetECHElementsState(int index, bool state)
    {
        switch (index)
        {
            case 0:
                {
                    ECH1Elements.gameObject.SetActive(state);
                }
                break;
            case 1:
                {
                    ECH2Elements.gameObject.SetActive(state);
                }
                break;
            case 2:
                {
                    ECH3Elements.gameObject.SetActive(state);
                }
                break;
            default:
                {
                    Debug.LogError("Invalid index sent to SetECHElementsState - " + index);
                }
                break;

        }
    }


    public void ApplyPCHBuffStatusAilment(int index, StatusAilments.Buffs buff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = PCH1BuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = PCH2BuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = PCH3BuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                if (TargetCollection[i].text == "")
                {
                    switch (buff)
                    {
                        case StatusAilments.Buffs.DEFENSE:
                            {
                                TargetCollection[i].text = "+Defense";
                                return;
                            }
                        case StatusAilments.Buffs.ATTACK:
                            {
                                TargetCollection[i].text = "+Attack";
                                return;
                            }
                    }
                }
            }
        }

        Debug.LogError("Error at ApplyPCHBuffStatusAilment - HUD");
    }
    public void ApplyPCHDebuffStatusAilment(int index, StatusAilments.Debuffs debuff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = PCH1DebuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = PCH2DebuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = PCH3DebuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                if (TargetCollection[i].text == "")
                {
                    switch (debuff)
                    {
                        case StatusAilments.Debuffs.DEFENSE:
                            {
                                TargetCollection[i].text = "-Defense";
                                return;
                            }
                        case StatusAilments.Debuffs.ATTACK:
                            {
                                TargetCollection[i].text = "-Attack";
                                return;
                            }
                    }
                }
            }
        }

        Debug.LogError("Error at ApplyPCHDebuffStatusAilment - HUD");
    }

    public void ApplyECHBuffStatusAilment(int index, StatusAilments.Buffs buff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = ECH1BuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = ECH2BuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = ECH3BuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                if (TargetCollection[i].text == "")
                {
                    switch (buff)
                    {
                        case StatusAilments.Buffs.DEFENSE:
                            {
                                TargetCollection[i].text = "+Defense";
                                return;
                            }
                        case StatusAilments.Buffs.ATTACK:
                            {
                                TargetCollection[i].text = "+Attack";
                                return;
                            }
                    }
                }
            }
        }

        Debug.LogError("Error at ApplyECHBuffStatusAilment - HUD");
    }
    public void ApplyECHDebuffStatusAilment(int index, StatusAilments.Debuffs debuff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = ECH1DebuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = ECH2DebuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = ECH3DebuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                if (TargetCollection[i].text == "")
                {
                    switch (debuff)
                    {
                        case StatusAilments.Debuffs.DEFENSE:
                            {
                                TargetCollection[i].text = "-Defense";
                                return;
                            }
                        case StatusAilments.Debuffs.ATTACK:
                            {
                                TargetCollection[i].text = "-Attack";
                                return;
                            }
                    }
                }
            }
        }

        Debug.LogError("Error at ApplyECHDebuffStatusAilment - HUD");
    }


    public void UapplyPCHBuffStatusAilment(int index, StatusAilments.Buffs buff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = PCH1BuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = PCH2BuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = PCH3BuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                switch (buff)
                {
                    case StatusAilments.Buffs.DEFENSE:
                        {
                            if (TargetCollection[i].text == "+Defense")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }break;
                    case StatusAilments.Buffs.ATTACK:
                        {
                            if (TargetCollection[i].text == "+Attack")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }break;
                }
            }
        }

        Debug.LogError("Error at UapplyPCHBuffStatusAilment - HUD");
    }
    public void UapplyPCHDebuffStatusAilment(int index, StatusAilments.Debuffs debuff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = PCH1DebuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = PCH2DebuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = PCH3DebuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                switch (debuff)
                {
                    case StatusAilments.Debuffs.DEFENSE:
                        {
                            if (TargetCollection[i].text == "-Defense")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }
                        break;
                    case StatusAilments.Debuffs.ATTACK:
                        {
                            if (TargetCollection[i].text == "-Attack")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }
                        break;
                }
            }
        }

        Debug.LogError("Error at UapplyPCHDebuffStatusAilment - HUD");
    }

    public void UapplyECHBuffStatusAilment(int index, StatusAilments.Buffs buff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = ECH1BuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = ECH2BuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = ECH3BuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                switch (buff)
                {
                    case StatusAilments.Buffs.DEFENSE:
                        {
                            if (TargetCollection[i].text == "+Defense")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }
                        break;
                    case StatusAilments.Buffs.ATTACK:
                        {
                            if (TargetCollection[i].text == "+Attack")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }
                        break;
                }
            }
        }

        Debug.LogError("Error at UapplyECHBuffStatusAilment - HUD");
    }
    public void UapplyECHDebuffStatusAilment(int index, StatusAilments.Debuffs debuff)
    {
        Text[] TargetCollection = new Text[2];

        switch (index)
        {
            case 0:
                {
                    TargetCollection = ECH1DebuffTexts;
                }
                break;
            case 1:
                {
                    TargetCollection = ECH2DebuffTexts;
                }
                break;
            case 2:
                {
                    TargetCollection = ECH3DebuffTexts;
                }
                break;
        }

        for (int i = 0; i < TargetCollection.Length; i++)
        {
            if (TargetCollection[i])
            {
                switch (debuff)
                {
                    case StatusAilments.Debuffs.DEFENSE:
                        {
                            if (TargetCollection[i].text == "-Defense")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }
                        break;
                    case StatusAilments.Debuffs.ATTACK:
                        {
                            if (TargetCollection[i].text == "-Attack")
                            {
                                TargetCollection[i].text = "";
                                return;
                            }
                        }
                        break;
                }
            }
        }

        Debug.LogError("Error at UapplyECHDebuffStatusAilment - HUD");
    }




    private void Awake()
    {
        SetupElements();
        SetupStatusAilments();
        SetupPCHStatusAilmentsTexts();
        SetupECHStatusAilmentsTexts();
        SetupHealthBars();
        SetupManaBars();
        SetupActionTexts();
        SetupPortraits();
        SetupAggroCountersTexts();
        SetupSuperBar();
    }
}
